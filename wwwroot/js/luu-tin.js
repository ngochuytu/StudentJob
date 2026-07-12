document.addEventListener("DOMContentLoaded", () => {
    const saveButton = document.getElementById("saveJobButton");
    const alertContainer = document.getElementById("saveJobAlert");

    if (!saveButton) {
        return;
    }

    saveButton.addEventListener("click", async () => {
        const jobId = saveButton.dataset.jobId;
        const isSaved = saveButton.dataset.isSaved === "true";

        const tokenInput = document.querySelector(
            'input[name="__RequestVerificationToken"]'
        );

        if (!jobId || !tokenInput) {
            showAlert(
                "danger",
                "Không thể xác định dữ liệu lưu tin."
            );
            return;
        }

        const endpoint = isSaved
            ? "/sinh-vien/api/bo-luu-tin"
            : "/sinh-vien/api/luu-tin";

        const formData = new FormData();
        formData.append("baiTuyenDungId", jobId);

        setLoading(true);
        clearAlert();

        try {
            const response = await fetch(endpoint, {
                method: "POST",
                headers: {
                    RequestVerificationToken:
                        tokenInput.value,
                    Accept: "application/json"
                },
                body: formData
            });

            const result = await response.json();

            if (!response.ok || result.success !== true) {
                throw new Error(
                    result.message ??
                    "Không thể cập nhật trạng thái lưu tin."
                );
            }

            const saved = result.data?.saved === true;

            saveButton.dataset.isSaved =
                saved.toString();

            updateButton(saved);
            showAlert("success", result.message);
        } catch (error) {
            showAlert(
                "danger",
                error.message ??
                "Đã xảy ra lỗi khi xử lý lưu tin."
            );
        } finally {
            setLoading(false);
        }
    });

    function updateButton(saved) {
        const textContainer =
            saveButton.querySelector(".save-job-text");

        if (!textContainer) {
            return;
        }

        if (saved) {
            saveButton.classList.remove(
                "btn-outline-primary"
            );
            saveButton.classList.add("btn-primary");

            textContainer.innerHTML = `
                <span>Đã lưu tin</span>
            `;
        } else {
            saveButton.classList.remove("btn-primary");
            saveButton.classList.add(
                "btn-outline-primary"
            );

            textContainer.innerHTML = `
                <span>Lưu tin</span>
            `;
        }
    }

    function setLoading(isLoading) {
        const normalContent =
            saveButton.querySelector(".save-job-text");

        const loadingContent =
            saveButton.querySelector(".save-job-loading");

        saveButton.disabled = isLoading;

        normalContent?.classList.toggle(
            "d-none",
            isLoading
        );

        loadingContent?.classList.toggle(
            "d-none",
            !isLoading
        );
    }

    function showAlert(type, message) {
        if (!alertContainer) {
            return;
        }

        alertContainer.innerHTML = `
            <div class="alert alert-${type} border-0 py-2 small"
                 role="alert">
                ${escapeHtml(message)}
            </div>
        `;
    }

    function clearAlert() {
        if (alertContainer) {
            alertContainer.innerHTML = "";
        }
    }

    function escapeHtml(value) {
        const element = document.createElement("div");
        element.textContent = value;
        return element.innerHTML;
    }
});