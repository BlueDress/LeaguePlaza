document.addEventListener("DOMContentLoaded", myQuestsMain());

function myQuestsMain() {
    const baseUrl = '/api/questapi/';

    const createQuestInputs = document.querySelectorAll('.create-quest-input-js');

    const titleInput = document.querySelector('#title');
    const descriptionTextarea = document.querySelector('#description');
    const rewardInput = document.querySelector('#reward');
    const typeSelect = document.querySelector('#type');
    const imageInput = document.querySelector('#image');

    const createQuestForm = document.querySelector('#create-quest');
    const createBtn = document.querySelector('#create-quest-btn');
    const updateBtn = document.querySelector('#update-quest-btn');
    const cardsAndPaginationHolder = document.querySelector('#cards-and-pagination');
    const questCardsContainer = document.querySelector('#quest-cards-container');

    let questId;

    createQuestInputs?.forEach(i => i.addEventListener('blur', e => handleFormElementInput(e.target)));
    typeSelect?.addEventListener('change', e => handleFormElementInput(e.target));
    imageInput?.addEventListener('blur', e => handleFormFileInput(e.target));
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

    function handleFormFileInput(element) {
        const imageSpan = element.nextElementSibling;
        hideErrorMessage(imageSpan);

        const file = element.files[0];

        if (file) {
            const fileType = file.type;
            const fileSize = file.size;

            if (!fileType.startsWith('image/')) {
                displayErrorMessage(imageSpan, 'Only image files can be uploaded');
                return;
            }

            if (fileSize > 5 * 1024 * 1024) {
                displayErrorMessage(imageSpan, 'Only files with size 5MB or less can be uploaded');
                return;
            }
        }
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

        if (formIsValid([titleInput, rewardInput, typeSelect], imageInput)) {
            const formData = new FormData();
            formData.append('title', titleInput.value);
            formData.append('description', descriptionTextarea.value);
            formData.append('rewardAmount', rewardInput.value);
            formData.append('type', typeSelect.value);
            formData.append('image', imageInput?.files[0]);

            const response = await fetch(baseUrl + 'createquest', {
                method: 'POST',
                body: formData,
            });

            if (response.status == 200) {
                const cardsAndPaginationHolderView = await response.text();
                cardsAndPaginationHolder.innerHTML = cardsAndPaginationHolderView;
                ShowFormMessage('success-message', 'The quest was created successfully');
                ClearInputs();
                ClearFilters();
            }

            if (response.status == 400) {
                ShowFormMessage('error-message', 'Something went wrong');
            }
        }

        createBtn.removeAttribute('disabled');
    }

    async function updateQuest(e) {
        if (formIsValid([titleInput, rewardInput, typeSelect], imageInput)) {
            updateBtn.setAttribute('disabled', 'disabled');

            const formData = new FormData();
            formData.append('id', questId);
            formData.append('title', titleInput.value);
            formData.append('description', descriptionTextarea.value);
            formData.append('rewardAmount', rewardInput.value);
            formData.append('type', typeSelect.value);
            formData.append('image', imageInput?.files[0]);

            const response = await fetch(baseUrl + 'updatequest', {
                method: 'PUT',
                body: formData,
            });

            if (response.status == 200) {
                createBtn.removeAttribute('disabled');
                const cardsAndPaginationHolderView = await response.text();
                cardsAndPaginationHolder.innerHTML = cardsAndPaginationHolderView;
                ShowFormMessage('success-message', 'The quest was updated successfully');
                ClearInputs();
                ClearFilters();
            }

            if (response.status == 400) {
                updateBtn.removeAttribute('disabled');
                ShowFormMessage('error-message', 'Something went wrong');
            }
        }
    }

    function formIsValid(inputs, imageInput) {
        let formIsValid = true;

        for (const input of inputs) {
            const span = input.nextElementSibling;

            if (elementInputIsValid(input)) {
                hideErrorMessage(span);
                continue;
            }

            displayErrorMessage(span, `${input.name} is required`);
            formIsValid = false;
        }

        const imageSpan = imageInput.nextElementSibling;
        hideErrorMessage(imageSpan);

        const file = imageInput.files[0];

        if (file) {
            const fileType = file.type;
            const fileSize = file.size;

            if (fileSize > 5 * 1024 * 1024) {
                displayErrorMessage(imageSpan, 'Only files with size 5MB or less can be uploaded');
                formIsValid = false;
                return formIsValid;
            }

            if (!fileType.startsWith('image/')) {
                displayErrorMessage(imageSpan, 'Only image files can be uploaded');
                formIsValid = false;
                return formIsValid;
            }
        }

        return formIsValid;
    }

    function ShowFormMessage(styleClass, message) {
        const formMessageElement = document.querySelector('.form-message-js');
        formMessageElement.classList.add(styleClass);
        formMessageElement.innerText = message;
        formMessageElement.classList.remove('display-none');

        setTimeout(() => {
            formMessageElement.classList.add('display-none');
            formMessageElement.classList.remove(styleClass);
            formMessageElement.innerText = '';
        }, 5000);
    }

    function ClearInputs() {
        titleInput.value = '';
        descriptionTextarea.value = '';
        rewardInput.value = '';
        typeSelect.value = '';
        imageInput.value = '';
    }

    function ClearFilters() {
        document.querySelectorAll('#status-filters input').forEach(f => f.checked = false);
        document.querySelectorAll('#type-filters input').forEach(f => f.checked = false);
        document.querySelector('#sort-by').value = 1;
        document.querySelector('#search').value = '';
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
            if (e.target.classList.contains('accept-btn-js')) {
                await acceptQuest(e);
            }
            if (e.target.classList.contains('abandon-btn-js')) {
                await abandonQuest(e);
            }
        }
    }

    function editBtnClick(e) {
        createQuestForm.scrollIntoView({ behavior: 'smooth' });

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

        if (response.status == 200) {
            const cardsAndPaginationHolderView = await response.text();
            cardsAndPaginationHolder.innerHTML = cardsAndPaginationHolderView;
            ClearInputs();
            ClearFilters();
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
            const cardsAndPaginationHolderView = await response.text();
            cardsAndPaginationHolder.innerHTML = cardsAndPaginationHolderView;
            ClearInputs();
            ClearFilters();
        }
    }

    async function acceptQuest(e) {
        const questId = e.target.parentElement.dataset.questId;

        const response = await fetch(baseUrl + 'acceptquest', {
            method: 'PUT',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify({ id: questId })
        });

        // TODO: handle response from server

        if (response.status == 200) {
            e.target.parentElement.appendChild(createElement('button', 'Abandon', { class: 'abandon-btn abandon-btn-js', type: 'button' }));
            e.target.parentElement.removeChild(e.target);
        }
    }

    async function abandonQuest(e) {
        const questId = e.target.parentElement.dataset.questId;

        const response = await fetch(baseUrl + 'abandonquest', {
            method: 'PUT',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify({ id: questId })
        });

        // TODO: handle response from server

        if (response.status == 200) {
            e.target.parentElement.appendChild(createElement('button', 'Accept', { class: 'accept-btn accept-btn-js', type: 'button' }));
            e.target.parentElement.removeChild(e.target);
        }
    }
}