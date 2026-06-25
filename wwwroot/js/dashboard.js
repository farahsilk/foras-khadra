// Dashboard: pulls JSON from the .NET analytics endpoints and draws the charts.

const PINE = '#14352a';
const MOSS = '#2e6b4f';
const SPROUT = '#8fd14f';
const PALETTE = ['#2e6b4f', '#8fd14f', '#14352a', '#5aa17a', '#bcd99a', '#3f7d5c', '#a3c46a', '#1d4636'];

const fmt = (n) => new Intl.NumberFormat('en-US').format(n);

async function getJson(url) {
    const res = await fetch(url);
    if (!res.ok) throw new Error(url + ' -> ' + res.status);
    return res.json();
}

async function loadOverview() {
    try {
        const o = await getJson('/api/analytics/overview');
        setStat('total', fmt(o.totalOpportunities));
        setStat('open', fmt(o.openOpportunities));
        setStat('visitors', fmt(o.latestMonthVisitors));
        setStat('engagement', o.avgEngagementRate + '%');
    } catch (e) { console.error(e); }
}

function setStat(key, value) {
    const el = document.querySelector(`[data-stat="${key}"]`);
    if (el) el.textContent = value;
}

async function loadVisitors() {
    const data = await getJson('/api/analytics/visitors');
    new Chart(document.getElementById('visitorsChart'), {
        type: 'bar',
        data: {
            labels: data.map(d => d.label),
            datasets: [{
                data: data.map(d => d.value),
                backgroundColor: MOSS,
                borderRadius: 6,
                maxBarThickness: 26
            }]
        },
        options: baseOptions({ yTicks: true })
    });
}

async function loadByType() {
    const data = await getJson('/api/analytics/by-type');
    new Chart(document.getElementById('typeChart'), {
        type: 'doughnut',
        data: {
            labels: data.map(d => d.label),
            datasets: [{
                data: data.map(d => d.value),
                backgroundColor: PALETTE,
                borderWidth: 2,
                borderColor: '#fff'
            }]
        },
        options: {
            responsive: true,
            cutout: '62%',
            plugins: {
                legend: { position: 'bottom', labels: { font: { family: 'IBM Plex Sans Arabic' }, boxWidth: 12, padding: 12 } }
            }
        }
    });
}

async function loadEngagement() {
    const data = await getJson('/api/analytics/engagement');
    new Chart(document.getElementById('engagementChart'), {
        type: 'bar',
        data: {
            labels: data.map(d => d.label),
            datasets: [{
                data: data.map(d => d.value),
                backgroundColor: SPROUT,
                borderRadius: 6,
                maxBarThickness: 40
            }]
        },
        options: baseOptions({ yTicks: true, suffix: '%' })
    });
}

async function loadTopOpportunities() {
    const tbody = document.querySelector('#topTable tbody');
    try {
        const data = await getJson('/api/analytics/top-opportunities?take=8');
        tbody.innerHTML = '';
        data.forEach(o => {
            const open = o.status === 'متاح';
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${escapeHtml(o.title)}</td>
                <td>${escapeHtml(o.type)}</td>
                <td><span class="pill ${open ? 'open' : 'closed'}">${o.status}</span></td>
                <td class="num">${fmt(o.views)}</td>`;
            tbody.appendChild(tr);
        });
    } catch (e) {
        tbody.innerHTML = '<tr><td colspan="4" class="muted">تعذّر تحميل البيانات</td></tr>';
        console.error(e);
    }
}

function baseOptions({ yTicks = false, suffix = '' } = {}) {
    return {
        responsive: true,
        plugins: { legend: { display: false } },
        scales: {
            x: { grid: { display: false }, ticks: { font: { family: 'IBM Plex Sans Arabic', size: 10 } } },
            y: {
                display: yTicks,
                grid: { color: '#eee' },
                ticks: { callback: (v) => v + suffix, font: { family: 'Space Grotesk' } }
            }
        }
    };
}

function escapeHtml(s) {
    return String(s).replace(/[&<>"']/g, c =>
        ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
}

loadOverview();
loadVisitors();
loadByType();
loadEngagement();
loadTopOpportunities();
