document.addEventListener("DOMContentLoaded", () => {
    const deleteButtons =
        document.querySelectorAll(".btn-delete-job");

    const deleteModalElement =
        document.getElementById("deleteJobModal");

    const confirmDeleteButton =
        document.getElementById("confirmDeleteJobButton");

    const deleteJobTitle =
        document.getElementById("deleteJobTitle");

    const alertContainer =
        document.getElementById("deleteAlertContainer");

    if (
        !deleteModalElement ||
        !confirmDeleteButton ||
        !deleteJobTitle
    ) {
        return;
    }

    const deleteModal =
        new bootstrap.Modal(deleteModalElement);

    let selectedJobId = null;

    deleteButtons.forEach(button => {
        button.addEventListener("click", () => {
            selectedJobId = button.dataset.jobId ?? null;

            deleteJobTitle.textContent =
                button.dataset.jobTitle ?? "";

            deleteModal.show();
        });
    });

    confirmDeleteButton.addEventListener(
        "click",
        async () => {
            if (!selectedJobId) {
                return;
            }

            const tokenInput = document.querySelector(
                'input[name="__RequestVerificationToken"]'
            );

            if (!tokenInput?.value) {
                showAlert(
                    "danger",
                    "Không tìm thấy mã xác thực biểu mẫu. Vui lòng tải lại trang."
                );

                deleteModal.hide();
                return;
            }

            setDeleteButtonLoading(true);

            try {
                const response = await fetch(
                    `/nha-tuyen-dung/api/xoa-tin/${selectedJobId}`,
                    {
                        method: "POST",
                        headers: {
                            Accept: "application/json",
                            RequestVerificationToken:
                                tokenInput.value
                        }
                    }
                );

                const result =
                    await readJsonResponse(response);

                if (!response.ok) {
                    throw new Error(
                        result?.message ??
                        `Không thể xóa tin tuyển dụng. Mã lỗi: ${response.status}.`
                    );
                }

                if (result?.success !== true) {
                    throw new Error(
                        result?.message ??
                        "Phản hồi từ máy chủ không hợp lệ."
                    );
                }

                const deletedRow =
                    document.getElementById(
                        `job-row-${selectedJobId}`
                    );

                let status = "";

                if (deletedRow) {
                    const statusBadge =
                        deletedRow.querySelector(
                            ".badge.rounded-pill"
                        );

                    status =
                        statusBadge?.textContent?.trim() ?? "";

                    deletedRow.remove();
                }

                updateTotalJobCount(status);
                showAlert("success", result.message);

                deleteModal.hide();
                selectedJobId = null;
            } catch (error) {
                showAlert(
                    "danger",
                    error instanceof Error
                        ? error.message
                        : "Đã xảy ra lỗi khi xóa tin tuyển dụng."
                );

                deleteModal.hide();
            } finally {
                setDeleteButtonLoading(false);
            }
        }
    );

    async function readJsonResponse(response) {
        const contentType =
            response.headers.get("content-type") ?? "";

        if (!contentType.includes("application/json")) {
            return null;
        }

        const responseText = await response.text();

        if (!responseText.trim()) {
            return null;
        }

        try {
            return JSON.parse(responseText);
        } catch {
            return null;
        }
    }

    function setDeleteButtonLoading(isLoading) {
        const normalContent =
            confirmDeleteButton.querySelector(
                ".delete-button-text"
            );

        const loadingContent =
            confirmDeleteButton.querySelector(
                ".delete-button-loading"
            );

        confirmDeleteButton.disabled = isLoading;

        normalContent?.classList.toggle(
            "d-none",
            isLoading
        );

        loadingContent?.classList.toggle(
            "d-none",
            !isLoading
        );
    }

    function updateTotalJobCount(status) {
        const totalJobCountElement =
            document.getElementById("totalJobCount");

        const approvedJobCountElement =
            document.getElementById(
                "approvedJobCount"
            );

        const pendingJobCountElement =
            document.getElementById(
                "pendingJobCount"
            );

        const rejectedJobCountElement =
            document.getElementById(
                "rejectedJobCount"
            );

        const remainingRows =
            document.querySelectorAll(
                "#jobTableBody tr"
            ).length;

        if (totalJobCountElement) {
            totalJobCountElement.textContent =
                remainingRows.toString();
        }

        if (status === "Đã duyệt") {
            decreaseCount(
                approvedJobCountElement
            );
        } else if (status === "Chờ duyệt") {
            decreaseCount(
                pendingJobCountElement
            );
        } else if (status === "Từ chối") {
            decreaseCount(
                rejectedJobCountElement
            );
        }

        if (remainingRows === 0) {
            window.location.reload();
        }
    }

    function decreaseCount(element) {
        if (!element) {
            return;
        }

        const current =
            Number.parseInt(
                element.textContent ?? "0",
                10
            ) || 0;

        element.textContent =
            Math.max(0, current - 1).toString();
    }

    function showAlert(type, message) {
        if (!alertContainer) {
            return;
        }

        alertContainer.innerHTML = `
            <div class="alert alert-${type} alert-dismissible fade show border-0 shadow-sm"
                 role="alert">
                ${escapeHtml(message)}
                <button type="button"
                        class="btn-close"
                        data-bs-dismiss="alert"
                        aria-label="Đóng">
                </button>
            </div>
        `;

        window.scrollTo({
            top: 0,
            behavior: "smooth"
        });
    }

    function escapeHtml(value) {
        const element =
            document.createElement("div");

        element.textContent =
            value ?? "";

        return element.innerHTML;
    }
});