document.addEventListener("DOMContentLoaded", myQuestsMain());

function myQuestsMain() {
    const baseUrl = '/api/questapi/';

    const inputs = document.querySelectorAll('input');

    const titleInput = document.querySelector('#title');
    const descriptionInput = document.querySelector('#description');
    const rewardInput = document.querySelector('#reward');
    const typeSelect = document.querySelector('#type');

    const createQuestForm = document.querySelector('#create-quest');
    const updateBtn = document.querySelector('#update-quest-btn');
    const questsHolder = document.querySelector('#quests-holder');

    let questId;

    inputs?.forEach(i => i.addEventListener('blur', e => handleFormElementInput(e.target)));
    typeSelect?.addEventListener('blur', e => handleFormElementInput(e.target));
    createQuestForm?.addEventListener('submit', e => createQuest(e));
    updateBtn?.addEventListener('click', e => updateQuest(e));
    questsHolder.addEventListener('click', e => handleQuestButtonClick(e));

    function handleFormElementInput(element) {
        const span = element.nextElementSibling;

        if (elementInputIsValid(element)) {
            hideErrorMessage(span);
            return;
        }

        displayErrorMessage(span, `${element.name} is required`);
    }

    function elementInputIsValid(element) {
        return !element.hasAttribute('required') || element.value;
    }

    function displayErrorMessage(element, message) {
        element.classList.add('error-message');
        element.classList.remove('display-none');
        element.innerText = message;
    }

    function hideErrorMessage(element) {
        element.classList.remove('error-message');
        element.classList.add('display-none');
        element.innerText = '';
    }

    async function createQuest(e) {
        e.preventDefault();

        if (formIsValid([titleInput, rewardInput, typeSelect])) {
            const newQuest = {
                title: titleInput.value,
                description: descriptionInput.value,
                rewardAmount: rewardInput.value,
                type: typeSelect.value
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

    async function updateQuest(e) {
        if (formIsValid([titleInput, rewardInput, typeSelect])) {
            const questToUpdate = {
                id: questId,
                title: titleInput.value,
                description: descriptionInput.value,
                rewardAmount: rewardInput.value,
                type: typeSelect.value
            }

            const response = await fetch(baseUrl + 'updatequest', {
                method: 'PUT',
                headers: {
                    'content-type': 'application/json',
                },
                body: JSON.stringify(questToUpdate),
            });
        }
    }

    function formIsValid(elements) {
        let formIsValid = true;

        for (const element of elements) {
            const span = element.nextElementSibling;

            if (elementInputIsValid(element)) {
                hideErrorMessage(span);
                break;
            }

            displayErrorMessage(span, `${element.name} is required`);
            formIsValid = false;
        }

        return formIsValid;
    }

    async function handleQuestButtonClick(e) {
        if (e.target) {
            if (e.target.classList.contains('edit-btn-js')) {
                editBtnClick(e);
            }
            if (e.target.classList.contains('complete-btn-js')) {
                await completeQuest(e);
            } if (e.target.classList.contains('abandon-btn-js')) {
                await abandonQuest(e);
            }
        }
    }

    function editBtnClick(e) {
        const ul = e.target.parentElement.children[0];

        titleInput.value = ul.children[1].textContent;
        descriptionInput.value = ul.children[2].textContent;
        rewardInput.value = parseFloat(ul.children[4].textContent.replace(',', '.'));
        typeSelect.value = getQuestTypeValue(ul.children[5].textContent);
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

        if (response.status == 200) {
            const questInfoElement = e.target.closest('.quest-info-js');
            questsHolder.removeChild(questInfoElement);
        }
    }
}