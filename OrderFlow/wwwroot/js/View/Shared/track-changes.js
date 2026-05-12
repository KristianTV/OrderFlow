function watchFormChanges(formId, buttonId) {
    const form = document.getElementById(formId);
    const saveBtn = document.getElementById(buttonId);

    if (!form || !saveBtn) return;

    const getSerializedState = () => new URLSearchParams(new FormData(form)).toString();
    const initialState = getSerializedState();

    const checkChanges = () => {
        const currentState = getSerializedState();
        saveBtn.disabled = (initialState === currentState);
    };

    form.addEventListener("input", checkChanges);
    form.addEventListener("change", checkChanges);

    checkChanges();
}