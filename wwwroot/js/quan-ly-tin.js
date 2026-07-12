document.addEventListener("DOMContentLoaded", () => {
    const deleteButtons = document.querySelectorAll(".btn-delete-job");
    const deleteModalElement = document.getElementById("deleteJobModal");
    const confirmDeleteButton = document.getElementById("confirmDeleteJobButton");
    const deleteJobTitle = document.getElementById("deleteJobTitle");
    const alertContainer = document.getElementById("deleteAlertContainer");
    if (
        !deleteModalElement ||
        !confirmDeleteButton ||
        !deleteJobTitle
    ) {
        return;
    }

    const deleteModal = new bootstrap.Modal(deleteModalElement);

    let selectedJobId = null;

    deleteButtons.forEach((button) => {
        button.addEventListener("click", () => {
            selectedJobId = button.dataset.jobId;
            deleteJobTitle.textContent = button.dataset.jobTitle ?? "";
            deleteModal.show();
        });
    });

    confirmDeleteButton.addEventListener("click", async () => {
        if (!selectedJobId) {
            return;
        }

        setDeleteButtonLoading(true);

        try {
            const response = await fetch(
                `/nha-tuyen-dung/api/xoa-tin/${selectedJobId}`,
                {
                    method: "POST",
                    headers: {
                        Accept: "application/json"
                    }
                }
            );

            const result = await response.json();

            if (!response.ok || result.success !== true) {
                throw new Error(
                    result.message ?? "Không thể xóa tin tuyển dụng."
                );
            }

            const deletedRow = document.getElementById(
                `job-row-${selectedJobId}`
            );

            let status = "";
            if (deletedRow) {
                const statusBadge = deletedRow.querySelector(".badge.rounded-pill");
                status = statusBadge ? statusBadge.textContent.trim() : "";
                deletedRow.remove();
            }

            updateTotalJobCount(status);
            showAlert("success", result.message);
            deleteModal.hide();
            selectedJobId = null;
        } catch (error) {
            showAlert(
                "danger",
                error.message ?? "Đã xảy ra lỗi khi xóa tin tuyển dụng."
            );

            deleteModal.hide();
        } finally {
            setDeleteButtonLoading(false);
        }
    });

    function setDeleteButtonLoading(isLoading) {
        const normalContent = confirmDeleteButton.querySelector(
            ".delete-button-text"
        );

        const loadingContent = confirmDeleteButton.querySelector(
            ".delete-button-loading"
        );

        confirmDeleteButton.disabled = isLoading;
        normalContent?.classList.toggle("d-none", isLoading);
        loadingContent?.classList.toggle("d-none", !isLoading);
    }

    function updateTotalJobCount(status) {
        const totalJobCountElement =
            document.getElementById("totalJobCount");
        const approvedJobCountElement =
            document.getElementById("approvedJobCount");
        const pendingJobCountElement =
            document.getElementById("pendingJobCount");
        const rejectedJobCountElement =
            document.getElementById("rejectedJobCount");

        const remainingRows =
            document.querySelectorAll("#jobTableBody tr").length;

        if (totalJobCountElement) {
            totalJobCountElement.textContent =
                remainingRows.toString();
        }

        if (status) {
            if (status === "Đã duyệt" && approvedJobCountElement) {
                const current = parseInt(approvedJobCountElement.textContent) || 0;
                approvedJobCountElement.textContent = Math.max(0, current - 1).toString();
            } else if (status === "Chờ duyệt" && pendingJobCountElement) {
                const current = parseInt(pendingJobCountElement.textContent) || 0;
                pendingJobCountElement.textContent = Math.max(0, current - 1).toString();
            } else if (status === "Từ chối" && rejectedJobCountElement) {
                const current = parseInt(rejectedJobCountElement.textContent) || 0;
                rejectedJobCountElement.textContent = Math.max(0, current - 1).toString();
            }
        }

        if (remainingRows === 0) {
            window.location.reload();
        }
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
        const element = document.createElement("div");
        element.textContent = value;
        return element.innerHTML;
    }
});