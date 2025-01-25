document.addEventListener("DOMContentLoaded", viewQuestMain());

function viewQuestMain() {
    const baseUrl = '/api/questapi/';

    const questInfo = document.querySelector('#quest-info');

    questInfo.addEventListener('click', e => handleButtonClick(e));

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
                    const btnHolder = document.querySelector('#button-holder');
                    btnHolder.appendChild(createElement('button', 'Abandon', { id: 'abandon-btn', type: 'button', 'data-quest-id': e.target.dataset.questId }));
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
                    const btnHolder = document.querySelector('#button-holder');
                    btnHolder.appendChild(createElement('button', 'Accept', { id: 'accept-btn', type: 'button', 'data-quest-id': e.target.dataset.questId }));
                    btnHolder.removeChild(e.target);
                }
            }
        }
    }
}