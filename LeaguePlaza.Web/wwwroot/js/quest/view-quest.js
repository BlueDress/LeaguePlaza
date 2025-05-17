document.addEventListener("DOMContentLoaded", viewQuestMain());

function viewQuestMain() {
    const baseUrl = '/api/questapi/';

    const btnHolder = document.querySelector('#button-holder');

    btnHolder.addEventListener('click', e => handleButtonClick(e));

    async function handleButtonClick(e) {
        if (e.target) {
            if (e.target.id == 'accept-btn') {
                const response = await fetch(baseUrl + 'acceptquest', {

                    method: 'PUT',
                    headers: {
                        'content-type': 'application/json',
                    },
                    body: JSON.stringify({ id: e.target.dataset.questId })
                });

                if (response.status == 200) {
                    btnHolder.appendChild(createElement('button', 'Abandon Quest', { id: 'abandon-btn', class: 'abandon-btn', type: 'button', 'data-quest-id': e.target.dataset.questId }));
                    btnHolder.removeChild(e.target);
                }
            }

            if (e.target.id == 'abandon-btn') {
                const response = await fetch(baseUrl + 'abandonQuest', {
                    method: 'PUT',
                    headers: {
                        'content-type': 'application/json',
                    },
                    body: JSON.stringify({ id: e.target.dataset.questId })
                });

                if (response.status == 200) {
                    btnHolder.appendChild(createElement('button', 'Accept Quest', { id: 'accept-btn', class: 'accept-btn', type: 'button', 'data-quest-id': e.target.dataset.questId }));
                    btnHolder.removeChild(e.target);
                }
            }
        }
    }
}