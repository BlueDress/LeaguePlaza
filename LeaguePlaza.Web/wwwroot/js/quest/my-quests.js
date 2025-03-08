document.addEventListener("DOMContentLoaded", myQuestsMain());

function myQuestsMain() {
    const baseUrl = '/api/questapi/';

    const createQuestInputs = document.querySelectorAll('.create-quest-input-js');

    const titleInput = document.querySelector('#title');
    const descriptionTextarea = document.querySelector('#description');
    const rewardInput = document.querySelector('#reward');
    const typeSelect = document.querySelector('#type');

    const createQuestForm = document.querySelector('#create-quest');
    const createBtn = document.querySelector('#create-quest-btn');
    const updateBtn = document.querySelector('#update-quest-btn');
    const cardsAndPaginationHolder = document.querySelector('#cards-and-pagination');
    const questCardsContainer = document.querySelector('#quest-cards-container');

    let questId;

    createQuestInputs?.forEach(i => i.addEventListener('blur', e => handleFormElementInput(e.target)));
    typeSelect?.addEventListener('blur', e => handleFormElementInput(e.target));
    createQuestForm?.addEventListener('submit', e => createQuest(e));
    updateBtn?.addEventListener('click', e => updateQuest(e));
    cardsAndPaginationHolder.addEventListener('click', e => handleQuestButtonClick(e));

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
        createBtn.setAttribute('disabled', 'disabled');
        e.preventDefault();

        if (formIsValid([titleInput, rewardInput, typeSelect])) {
            const newQuest = {
                title: titleInput.value,
                description: descriptionTextarea.value,
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

            if (response.status == 200) {
                const cardsAndPaginationHolderView = await response.text();
                cardsAndPaginationHolder.innerHTML = cardsAndPaginationHolderView;
                ClearInputs();
                // TODO: clear filters
            }

            // TODO: handle server error
        }

        createBtn.removeAttribute('disabled');
    }

    async function updateQuest(e) {
        if (formIsValid([titleInput, rewardInput, typeSelect])) {
            const questToUpdate = {
                id: questId,
                title: titleInput.value,
                description: descriptionTextarea.value,
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

            // TODO: update quest, clear inputs, clear filters
            // TODO: handle response from server

            if (response.status == 200) {
                createBtn.removeAttribute('disabled');
                updateBtn.setAttribute('disabled', 'disabled');
            }
        }
    }

    function formIsValid(elements) {
        let formIsValid = true;

        for (const element of elements) {
            const span = element.nextElementSibling;

            if (elementInputIsValid(element)) {
                hideErrorMessage(span);
                continue;
            }

            displayErrorMessage(span, `${element.name} is required`);
            formIsValid = false;
        }

        return formIsValid;
    }

    function ClearInputs() {
        titleInput.value = '';
        descriptionTextarea.value = '';
        rewardInput.value = '';
        typeSelect.value = '';
    }

    async function handleQuestButtonClick(e) {
        if (e.target) {
            if (e.target.classList.contains('edit-btn-js')) {
                editBtnClick(e);
            }
            if (e.target.classList.contains('remove-btn-js')) {
                await removeQuest(e);
            }
            if (e.target.classList.contains('complete-btn-js')) {
                await completeQuest(e);
            }
            if (e.target.classList.contains('abandon-btn-js')) {
                await abandonQuest(e);
            }
        }
    }

    function editBtnClick(e) {
        // TODO: scroll to form
        const questInfoEl = e.target.closest('.quest-info-js');
        const questMetadata = questInfoEl.children[2];

        titleInput.value = questInfoEl.children[0].textContent;
        descriptionTextarea.value = questInfoEl.children[1].textContent;
        rewardInput.value = parseFloat(questMetadata.children[2].textContent.replace(',', '.'));
        typeSelect.value = getQuestTypeValue(questMetadata.children[1].textContent);
        questId = e.target.parentElement.dataset.questId;

        createBtn.setAttribute('disabled', 'disabled');
        updateBtn.removeAttribute('disabled')
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

    async function removeQuest(e) {
        const questId = e.target.parentElement.dataset.questId;

        const response = await fetch(baseUrl + 'removequest', {
            method: 'DELETE',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify({ id: questId })
        });

        // TODO: handle response from server

        if (response.status === 200) {
            const questCardElement = e.target.closest('.quest-card-js');
            questCardsContainer.removeChild(questCardElement);
        }
    }

    async function completeQuest(e) {
        const questId = e.target.parentElement.dataset.questId;

        const response = await fetch(baseUrl + 'completequest', {
            method: 'PUT',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify({ id: questId })
        });

        // TODO: handle response from server

        if (response.status === 200) {
            const questInfoDiv = e.target.closest('.quest-info-js');
            questInfoDiv.removeChild(e.target.parentNode);
        }
    }

    async function abandonQuest(e) {
        const questId = e.target.parentElement.dataset.questId;

        const response = await fetch(baseUrl + 'abandonQuest', {
            method: 'PUT',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify({ id: questId })
        });

        // TODO: handle response from server

        if (response.status == 200) {
            const questCardElement = e.target.closest('.quest-card-js');
            questCardsContainer.removeChild(questCardElement);
        }
    }
}