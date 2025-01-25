document.addEventListener("DOMContentLoaded", viewQuestMain());

function viewQuestMain() {
    const baseUrl = '/api/questapi/';

    const acceptBtn = document.querySelector('#accept-btn-js');

    acceptBtn?.addEventListener('click', e => acceptQuest(e));

    async function acceptQuest(e) {
        const response = await fetch(baseUrl + 'acceptquest', {
            method: 'PUT',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify({ id: e.target.dataset.questId })
        });
    }
}