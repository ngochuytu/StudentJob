document.addEventListener("DOMContentLoaded", () => {
    const savedJobsList =
        document.getElementById("savedJobsList");

    const alertContainer =
        document.getElementById("savedJobsAlert");

    if (!savedJobsList) {
        return;
    }

    savedJobsList.addEventListener("click", async event => {
        const button = event.target.closest(
            ".saved-job-remove-button"
        );

        if (!button) {
            return;
        }

        const jobId = button.dataset.jobId;

        const tokenInput = document.querySelector(
            'input[name="__RequestVerificationToken"]'
        );

        if (!jobId || !tokenInput) {
            showAlert(
                "danger",
                "Không thể xác định tin tuyển dụng."
            );

            return;
        }

        button.disabled = true;

        const originalContent = button.innerHTML;

        button.innerHTML = `
            <span class="spinner-border spinner-border-sm"
                  aria-hidden="true">
            </span>
        `;

        try {
            const formData = new FormData();

            formData.append(
                "baiTuyenDungId",
                jobId
            );

            const response = await fetch(
                "/sinh-vien/api/bo-luu-tin",
                {
                    method: "POST",
                    headers: {
                        RequestVerificationToken:
                            tokenInput.value,
                        Accept: "application/json"
                    },
                    body: formData
                }
            );

            const result = await response.json();

            if (!response.ok ||
                result.success !== true) {
                throw new Error(
                    result.message ??
                    "Không thể bỏ lưu tin tuyển dụng."
                );
            }

            const savedJobItem = button.closest(
                ".saved-job-item"
            );

            savedJobItem?.remove();

            showAlert(
                "success",
                result.message
            );

            updateSavedJobCount();

            if (!savedJobsList.querySelector(
                    ".saved-job-item")) {
                window.location.reload();
            }
        } catch (error) {
            button.disabled = false;
            button.innerHTML = originalContent;

            showAlert(
                "danger",
                error.message ??
                "Đã xảy ra lỗi khi bỏ lưu tin."
            );
        }
    });

    function updateSavedJobCount() {
        const countElement =
            document.querySelector(
                ".saved-jobs-count strong"
            );

        if (!countElement) {
            return;
        }

        const currentCount =
            savedJobsList.querySelectorAll(
                ".saved-job-item"
            ).length;

        countElement.textContent =
            currentCount.toString();
    }

    function showAlert(type, message) {
        if (!alertContainer) {
            return;
        }

        const element =
            document.createElement("div");

        element.className =
            `alert alert-${type} border-0`;

        element.setAttribute(
            "role",
            "alert"
        );

        element.textContent = message;

        alertContainer.replaceChildren(element);
    }
});