document.addEventListener("DOMContentLoaded", myQuestsMain());

function myQuestsMain() {
    const baseUrl = '/api/questapi/';

    const titleInput = document.querySelector('#title');
    const descriptionInput = document.querySelector('#description');
    const rewardInput = document.querySelector('#reward');
    const typeInput = document.querySelector('#type');

    const createQuestForm = document.querySelector('#create-quest');
    const updateBtn = document.querySelector('#update-quest-btn');
    const editBtns = document.querySelectorAll('.edit-btn-js');
    const completeBtns = document.querySelectorAll('.complete-btn-js');
    const abandonBtns = document.querySelectorAll('.abandon-btn-js');

    let questId;

    createQuestForm.addEventListener('submit', e => createQuest(e));
    updateBtn.addEventListener('click', e => updateQuest(e));
    editBtns.forEach(eb => eb.addEventListener('click', e => editBtnClick(e)));
    completeBtns.forEach(cb => cb.addEventListener('click', e => completeQuest(e)));
    abandonBtns.forEach(ab => ab.addEventListener('click', e => abandonQuest(e)));

    async function createQuest(e) {
        e.preventDefault();

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

    async function updateQuest(e) {
        const questToUpdate = {
            id: questId,
            title: titleInput.value,
            description: descriptionInput.value,
            rewardAmount: rewardInput.value,
            type: typeInput.value
        }

        const response = await fetch(baseUrl + 'updatequest', {
            method: 'PUT',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify(questToUpdate),
        });
    }

    function editBtnClick(e) {
        const ul = e.target.parentElement.children[0];

        titleInput.value = ul.children[1].textContent;
        descriptionInput.value = ul.children[2].textContent;
        rewardInput.value = parseFloat(ul.children[4].textContent.replace(',', '.'));
        typeInput.value = getQuestTypeValue(ul.children[5].textContent);
        questId = ul.children[0].textContent;
    }

    function getQuestTypeValue(questType) {
        let value;

        switch (questType) {
            case 'Monster Hunt': value = 1; break;
            case 'Gathering': value = 2; break;
            case 'Escort': value = 3; break;
        }

        return value;
    }

    async function completeQuest(e) {
        const ul = e.target.parentElement.children[0];

        const response = await fetch(baseUrl + 'completequest', {
            method: 'PUT',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify({ id: ul.children[0].textContent })
        });
    }

    async function abandonQuest(e) {
        const ul = e.target.parentElement.children[0];

        const response = await fetch(baseUrl + 'abandonQuest', {
            method: 'PUT',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify({ id: ul.children[0].textContent })
        });
    }
}