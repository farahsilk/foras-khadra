// Chat: single-provider mode (/message) or compare mode (/compare across all keys).

const log = document.getElementById('chatLog');
const form = document.getElementById('chatForm');
const input = document.getElementById('chatText');
const sendBtn = document.getElementById('sendBtn');
const suggestions = document.getElementById('suggestions');
const providerSelect = document.getElementById('providerSelect');
const compareToggle = document.getElementById('compareToggle');

const history = []; // [{ role, content }]

// Populate the provider dropdown from the server.
(async function loadProviders() {
    try {
        const list = await (await fetch('/api/chat/providers')).json();
        providerSelect.innerHTML = '';
        list.forEach(p => {
            const opt = document.createElement('option');
            opt.value = p.name;
            opt.textContent = p.configured ? p.name : `${p.name} (لا يوجد مفتاح)`;
            opt.disabled = !p.configured;
            providerSelect.appendChild(opt);
        });
        const firstConfigured = list.find(p => p.configured);
        if (firstConfigured) providerSelect.value = firstConfigured.name;
    } catch (e) { console.error(e); }
})();

compareToggle.addEventListener('change', () => {
    providerSelect.disabled = compareToggle.checked;
});

function addBubble(text, who, label) {
    const wrap = document.createElement('div');
    wrap.className = 'msg ' + (who === 'user' ? 'msg-user' : 'msg-bot');
    const bubble = document.createElement('div');
    bubble.className = 'bubble';
    if (label) {
        const tag = document.createElement('span');
        tag.className = 'provider-tag';
        tag.textContent = label;
        bubble.appendChild(tag);
    }
    const body = document.createElement('span');
    body.textContent = text;
    bubble.appendChild(body);
    wrap.appendChild(bubble);
    log.appendChild(wrap);
    log.scrollTop = log.scrollHeight;
    return { bubble, body };
}

async function ask(message) {
    if (!message.trim()) return;
    addBubble(message, 'user');
    input.value = '';
    sendBtn.disabled = true;

    if (compareToggle.checked) {
        await runCompare(message);
    } else {
        await runSingle(message);
    }

    sendBtn.disabled = false;
    input.focus();
    log.scrollTop = log.scrollHeight;
}

async function runSingle(message) {
    history.push({ role: 'user', content: message });
    const provider = providerSelect.value;
    const { body } = addBubble('… يفكّر', 'bot', provider);
    body.parentElement.classList.add('typing');

    try {
        const res = await fetch('/api/chat/message?provider=' + encodeURIComponent(provider), {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ message, history: history.slice(0, -1) })
        });
        const data = await res.json();
        body.parentElement.classList.remove('typing');
        if (data.success) {
            body.previousSibling.textContent = `${data.provider} · ${data.elapsedMs}ms`;
            body.textContent = data.answer;
            history.push({ role: 'assistant', content: data.answer });
        } else {
            body.textContent = '⚠️ ' + (data.error || 'حدث خطأ.');
        }
    } catch (e) {
        body.parentElement.classList.remove('typing');
        body.textContent = '⚠️ تعذّر الاتصال بالخادم.';
        console.error(e);
    }
}

async function runCompare(message) {
    // Compare mode is stateless (no shared history) so the contest is fair.
    const grid = document.createElement('div');
    grid.className = 'compare-grid';
    log.appendChild(grid);

    const placeholder = document.createElement('div');
    placeholder.className = 'muted compare-loading';
    placeholder.textContent = 'يسأل كل النماذج…';
    grid.appendChild(placeholder);

    try {
        const res = await fetch('/api/chat/compare', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ message, history: [] })
        });
        const results = await res.json();
        grid.innerHTML = '';
        results.forEach(r => {
            const card = document.createElement('div');
            card.className = 'compare-card' + (r.success ? '' : ' is-error');
            const head = document.createElement('div');
            head.className = 'compare-head';
            head.innerHTML = `<strong>${r.provider || 'غير معروف'}</strong><span>${r.success ? r.elapsedMs + 'ms' : 'خطأ'}</span>`;
            const text = document.createElement('div');
            text.className = 'compare-text';
            text.textContent = r.success ? r.answer : (r.error || 'فشل الطلب');
            card.appendChild(head);
            card.appendChild(text);
            grid.appendChild(card);
        });
    } catch (e) {
        grid.innerHTML = '<div class="muted">⚠️ تعذّر الاتصال بالخادم.</div>';
        console.error(e);
    }
}

form.addEventListener('submit', (e) => { e.preventDefault(); ask(input.value); });
suggestions.addEventListener('click', (e) => {
    if (e.target.classList.contains('chip')) ask(e.target.textContent);
});
