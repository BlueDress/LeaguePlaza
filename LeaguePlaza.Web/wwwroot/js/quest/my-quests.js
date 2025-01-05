document.addEventListener("DOMContentLoaded", myQuestsMain());

function myQuestsMain() {
    const baseUrl = '/api/questapi/'

    const createQuestForm = document.querySelector('#create-quest');

    createQuestForm.addEventListener('submit', e => createQuest(e));

    async function createQuest(e) {
        e.preventDefault();

        const titleInput = document.querySelector('#title');
        const descriptionInput = document.querySelector('#description');
        const rewardInput = document.querySelector('#reward');
        const typeInput = document.querySelector('#type');

        const newQuest = {
            title: titleInput.value,
            description: descriptionInput.value,
            rewardAmount: rewardInput.value,
            type: typeInput.value
        }

        const response = await fetch(baseUrl + 'createquest', {
            method: 'POST',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify(newQuest),
        });
    }
}