document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("applicationForm");
    const submitButton = document.getElementById(
        "submitApplicationButton"
    );
    const useDefaultCv = document.getElementById("useDefaultCv");
    const uploadCvGroup = document.getElementById("uploadCvGroup");
    const fileInput = document.querySelector(
        'input[name="UngTuyen.TepCV"]'
    );
    const alertContainer = document.getElementById(
        "applicationAlert"
    );
    const modalElement = document.getElementById(
        "applicationModal"
    );

    if (!form || !submitButton || !modalElement) {
        return;
    }

    updateCvUploadVisibility();

    useDefaultCv?.addEventListener(
        "change",
        updateCvUploadVisibility
    );

    submitButton.addEventListener("click", async () => {
        clearAlert();

        const validationMessage = validateForm();

        if (validationMessage) {
            showAlert("danger", validationMessage);
            return;
        }

        const tokenInput = form.querySelector(
            'input[name="__RequestVerificationToken"]'
        );

        const jobIdInput = form.querySelector(
            'input[name="UngTuyen.FK_IdBaiTuyenDung"]'
        );

        const coverLetterInput = form.querySelector(
            'textarea[name="UngTuyen.sThuXinViec"]'
        );

        if (!tokenInput || !jobIdInput) {
            showAlert(
                "danger",
                "Không thể xác định dữ liệu nộp hồ sơ."
            );
            return;
        }

        const formData = new FormData();

        formData.append(
            "FK_IdBaiTuyenDung",
            jobIdInput.value
        );

        formData.append(
            "SuDungCVMacDinh",
            useDefaultCv?.checked ? "true" : "false"
        );

        formData.append(
            "sThuXinViec",
            coverLetterInput?.value ?? ""
        );

        if (
            !useDefaultCv?.checked &&
            fileInput?.files?.length
        ) {
            formData.append(
                "TepCV",
                fileInput.files[0]
            );
        }

        setLoading(true);

        try {
            const response = await fetch(
                "/sinh-vien/api/ung-tuyen",
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

            if (!response.ok || result.success !== true) {
                throw new Error(
                    result.message ??
                    "Không thể nộp hồ sơ ứng tuyển."
                );
            }

            showAlert("success", result.message);

            submitButton.disabled = true;

            setTimeout(() => {
                window.location.reload();
            }, 1200);
        } catch (error) {
            showAlert(
                "danger",
                error.message ??
                "Đã xảy ra lỗi khi nộp hồ sơ."
            );
        } finally {
            setLoading(false);
        }
    });

    function updateCvUploadVisibility() {
        if (!uploadCvGroup) {
            return;
        }

        const usingDefaultCv =
            useDefaultCv?.checked === true;

        uploadCvGroup.classList.toggle(
            "d-none",
            usingDefaultCv
        );

        if (fileInput) {
            fileInput.disabled = usingDefaultCv;

            if (usingDefaultCv) {
                fileInput.value = "";
            }
        }
    }

    function validateForm() {
        const usingDefaultCv =
            useDefaultCv?.checked === true;

        if (usingDefaultCv) {
            return null;
        }

        const selectedFile =
            fileInput?.files?.[0];

        if (!selectedFile) {
            return "Vui lòng chọn CV để tải lên.";
        }

        const allowedExtensions = [
            ".pdf",
            ".docx"
        ];

        const fileName =
            selectedFile.name.toLowerCase();

        const hasValidExtension =
            allowedExtensions.some(extension =>
                fileName.endsWith(extension)
            );

        if (!hasValidExtension) {
            return "CV chỉ chấp nhận định dạng PDF hoặc DOCX.";
        }

        const maximumFileSize =
            5 * 1024 * 1024;

        if (selectedFile.size > maximumFileSize) {
            return "Dung lượng CV không được vượt quá 5 MB.";
        }

        return null;
    }

    function setLoading(isLoading) {
        const normalContent =
            submitButton.querySelector(
                ".application-submit-text"
            );

        const loadingContent =
            submitButton.querySelector(
                ".application-submit-loading"
            );

        submitButton.disabled = isLoading;

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
            <div class="alert alert-${type} border-0"
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