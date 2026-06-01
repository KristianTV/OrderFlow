function initSonarConnection(notificationID) {

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/message")
        .withAutomaticReconnect()
        .build();

    connection.on("MessageAdded", (msg) => {
        removeEmptyState();
        const list = getOrCreateList();
        list.appendChild(buildMessageRow(msg));
        scrollMessagesToBottom();
        showToast('New message received.', 'success');
    });

    connection.on("MessageEdited", ({ messageID, content }) => {
        const li = document.querySelector(`li[data-message-id="${messageID}"]`);
        if (li) {
            li.querySelector('.message-content').textContent = content;
            li.querySelector('[data-bs-target="#editMessageModal"]')
              .setAttribute('data-message-content', content);
        }
    });

    connection.on("MessageDeleted", (messageID) => {
        const li = document.querySelector(`li[data-message-id="${messageID}"]`);
        if (li) li.remove();
        if (!document.querySelector('#messagesContainer li')) showEmptyState();
    });


    connection.start()
        .then(() => connection.invoke("JoinNotification", notificationID))
        .catch(err => console.error("SignalR connection error:", err));
}

function buildMessageRow(msg) {
    const li = createElement('li', {
        className: `list-group-item ${msg.isRead ? "" : "list-group-item-light"}`,
        dataset: { messageId: msg.messageID }
    });

    const wrapper = createElement('div', { className: 'd-flex justify-content-between align-items-start' }, li);
    const contentBody = createElement('div', { className: 'flex-grow-1 me-3' }, wrapper);

    const header = createElement('div', { className: 'd-flex align-items-center mb-1' }, contentBody);
    createElement('i', { className: 'fas fa-user-circle me-2 text-secondary' }, header);
    createElement('strong', { textContent: msg.senderName }, header);

    const timeSmall = createElement('small', { className: 'text-muted ms-3' }, header);
    createElement('i', { className: 'far fa-clock me-1' }, timeSmall);
    const date = new Date(msg.sentAt);
    const formattedDate = String(date.getDate()).padStart(2, '0') + '-' +
        String(date.getMonth() + 1).padStart(2, '0') + '-' +
        date.getFullYear() + ' ' +
        String(date.getHours()).padStart(2, '0') + ':' +
        String(date.getMinutes()).padStart(2, '0');

    timeSmall.appendChild(document.createTextNode(formattedDate));

    if (!msg.isRead) {
        createElement('span', { className: 'badge bg-primary ms-2', textContent: 'New' }, header);
    }

    createElement('p', {
        className: 'mb-0 ps-4 message-content',
        textContent: msg.content
    }, contentBody);

    if (msg.senderID === currentUserId) {
        const actions = createElement('div', { className: 'd-flex gap-2 flex-shrink-0' }, wrapper);

        const editBtn = createElement('button', {
            type: 'button',
            className: 'btn btn-sm btn-outline-primary',
            dataset: {
                bsToggle: 'modal',
                bsTarget: '#editMessageModal',
                messageId: msg.messageID,
                messageContent: msg.content
            }
        }, actions);
        createElement('i', { className: 'fas fa-edit' }, editBtn);

        const deleteBtn = createElement('button', {
            type: 'button',
            className: 'btn btn-sm btn-outline-danger',
            dataset: {
                bsToggle: 'modal',
                bsTarget: '#deleteMessageModal',
                messageId: msg.messageID
            }
        }, actions);
        createElement('i', { className: 'fas fa-trash-alt' }, deleteBtn);
    }

    return li;
}

function getOrCreateList() {
    let list = document.querySelector('#messagesContainer ul');
    if (!list) {
        const container = document.getElementById('messagesContainer');
        container.innerHTML = ''; 
        list = createElement('ul', { className: 'list-group list-group-flush' }, container);
    }
    return list;
}

/** Removes the "no messages" empty-state element if present. */
function removeEmptyState() {
    const empty = document.querySelector('#messagesContainer .text-center');
    if (empty) empty.remove();
}

/** Replaces the message list with the empty-state placeholder. */
function showEmptyState() {
    const container = document.getElementById('messagesContainer');
    container.innerHTML = '';
    const div = createElement('div', { className: 'p-4 text-center text-muted fst-italic' }, container);
    createElement('i', { className: 'far fa-comment-dots fa-2x mb-2 d-block' }, div);
    div.appendChild(document.createTextNode('No messages yet. Be the first to add one.'));
}

/**
 * Scrolls #messagesContainer to the very bottom so the latest message
 * is always visible after a new one is added.
 */
function scrollMessagesToBottom() {
    const container = document.getElementById('messagesContainer');
    if (container) {
        container.scrollTop = container.scrollHeight;
    }
}
