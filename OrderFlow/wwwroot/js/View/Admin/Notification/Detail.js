
document.addEventListener('DOMContentLoaded', () => {

    // ── Guard: only run message-panel logic when panel exists ──

    const messagesEnabled = !!document.getElementById('messagesContainer');

    if (!messagesEnabled) {
        return;
    }

        initSonarConnection(notificationID);
        scrollMessagesToBottom();

        // ── CRUD form submissions──

        document.getElementById('addMessageForm')?.addEventListener('submit', async e => {
            e.preventDefault();
            const ok = await submitForm('addMessageForm', 'addMessageModal');
            if (ok) {
                document.getElementById('addMessageContent').value = '';
            }
        });

        document.getElementById('editMessageForm')?.addEventListener('submit', async e => {
            e.preventDefault();
            await submitForm('editMessageForm', 'editMessageModal');
        });

        document.getElementById('deleteMessageForm')?.addEventListener('submit', async e => {
            e.preventDefault();
            await submitForm('deleteMessageForm', 'deleteMessageModal');
        });

        // ── Populate edit / delete modals on button click ──

        document.getElementById('messagesContainer').addEventListener('click', e => {
            const editBtn = e.target.closest('[data-bs-target="#editMessageModal"]');
            const deleteBtn = e.target.closest('[data-bs-target="#deleteMessageModal"]');

            if (editBtn) {
                const msgId = editBtn.dataset.messageId;
                const editForm = document.getElementById('editMessageForm');
                const url = new URL(editForm.action, window.location.origin);
                url.searchParams.set('messageId', msgId);
                editForm.action = url.toString();

                document.getElementById('editMessageContent').value =
                    editBtn.dataset.messageContent;
            }

            if (deleteBtn) {
                document.getElementById('deleteMessageID').value =
                    deleteBtn.dataset.messageId;
            }
        });
});

// ── Shared helpers ────────────────────────────────────────────

/**
 * Submits a form via fetch and hides its modal on success.
 *
 * @param {string} formId   – id of the <form> element
 * @param {string} modalId  – id of the parent Bootstrap modal
 * @returns {Promise<boolean>} true if the request succeeded
 */
async function submitForm(formId, modalId) {
    const form = document.getElementById(formId);
    try {
        const res = await fetch(form.action, {
            method:  'POST',
            body:    new FormData(form),
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });

        if (res.ok) {
            const modalEl = document.getElementById(modalId);
            const modal   = bootstrap.Modal.getInstance(modalEl);
            if (modal) modal.hide();

            // ── Overlay bug fix: clean up immediately after hiding ──
            document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
            document.body.classList.remove('modal-open');
            document.body.style.removeProperty('overflow');
            document.body.style.removeProperty('padding-right');

            return true;
        } else {
            showToast('Something went wrong. Please try again.', 'danger');
            return false;
        }
    } catch (err) {
        console.error('Form submit error:', err);
        showToast('Network error. Please try again.', 'danger');
        return false;
    }
}

/**
 * Displays a Bootstrap toast notification.
 *
 * @param {string} message – text to display
 * @param {'success'|'danger'|'warning'|'info'} type – Bootstrap colour variant
 */
function showToast(message, type = 'success') {
    const container = document.getElementById('toastContainer');
    if (!container) return;

    const toastEl = createElement('div', {
        id: 'toast-' + Date.now(),
        className: `toast align-items-center text-bg-${type} border-0`,
        role: 'alert',
        ariaLive: 'assertive',
        ariaAtomic: 'true'
    }, container);

    const flex = createElement('div', { className: 'd-flex' }, toastEl);
    createElement('div', { className: 'toast-body', textContent: message }, flex);

    const closeBtn = createElement('button', {
        type: 'button',
        className: 'btn-close btn-close-white me-2 m-auto',
        dataset: { bsDismiss: 'toast' },
        ariaLabel: 'Close'
    }, flex);

    const toast = new bootstrap.Toast(toastEl, { delay: 3000 });
    toast.show();

    // Clean up DOM after hide
    toastEl.addEventListener('hidden.bs.toast', () => toastEl.remove());
}