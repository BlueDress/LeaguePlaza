document.addEventListener("DOMContentLoaded", adminMain());

function adminMain() {
    const baseUrl = '/api/adminapi/';

    const mountNameInput = document.querySelector('#name');
    const mountDescriptionTextarea = document.querySelector('#description');
    const mountRentPriceInput = document.querySelector('#rent-price');
    const mountTypeSelect = document.querySelector('#mount-type');
    const mountImageInput = document.querySelector('#image');

    const createMountForm = document.querySelector('#create-mount');
    const createMountBtn = document.querySelector('#create-mount-btn');
    const updateMountBtn = document.querySelector('#update-mount-btn');
    const cardsAndPaginationHolder = document.querySelector('#cards-and-pagination');

    const pageName = document.querySelector('.container').dataset.pageName;

    let mountId;

    createMountForm?.addEventListener('submit', e => createMount(e));
    updateMountBtn?.addEventListener('click', e => updateMount(e));
    cardsAndPaginationHolder.addEventListener('click', e => handleMountButtonClick(e));
    cardsAndPaginationHolder.addEventListener('click', e => handlePaginationClick(e));

    async function createMount(e) {
        createMountBtn.setAttribute('disabled', 'disabled');
        e.preventDefault();

        const formData = new FormData();
        formData.append('name', mountNameInput.value);
        formData.append('description', mountDescriptionTextarea.value);
        formData.append('rentPrice', mountRentPriceInput.value);
        formData.append('mountType', mountTypeSelect.value);
        formData.append('image', mountImageInput?.files[0]);

        const response = await fetch(baseUrl + 'createmount', {
            method: 'POST',
            body: formData,
        });

        if (response.status == 200) {
            const cardsAndPaginationHolderView = await response.text();
            cardsAndPaginationHolder.innerHTML = cardsAndPaginationHolderView;
            ShowFormMessage('success-message', 'The mount was created successfully');
            ClearMountInputs();
        }

        createMountBtn.removeAttribute('disabled');
    }

    async function updateMount(e) {
        updateMountBtn.setAttribute('disabled', 'disabled');

        const formData = new FormData();
        formData.append('id', mountId);
        formData.append('description', mountDescriptionTextarea.value);
        formData.append('rentPrice', mountRentPriceInput.value);
        formData.append('mountType', mountTypeSelect.value);
        formData.append('image', mountImageInput?.files[0]);

        const response = await fetch(baseUrl + 'updatemount', {
            method: 'PUT',
            body: formData,
        });

        if (response.status == 200) {
            createMountForm.removeAttribute('disabled');
            const cardsAndPaginationHolderView = await response.text();
            cardsAndPaginationHolder.innerHTML = cardsAndPaginationHolderView;
            ShowFormMessage('success-message', 'The mount was updated successfully');
            ClearMountInputs();
        }
    }

    async function handleMountButtonClick(e) {
        if (e.target) {
            if (e.target.classList.contains('admin-mount-edit-button-js')) {
                mountEditBtnClick(e);
            }
            if (e.target.classList.contains('admin-mount-delete-button-js')) {
                await deleteMount(e);
            }
        }
    }

    function mountEditBtnClick(e) {
        createMountForm.scrollIntoView({ behavior: 'smooth' });

        const mountInfoEl = e.target.closest('.admin-mount-card-js').querySelector('.admin-mount-card-info-js');

        mountNameInput.value = mountInfoEl.children[0].textContent;
        mountDescriptionTextarea.value = mountInfoEl.children[1].textContent;
        mountRentPriceInput.value = parseFloat(mountInfoEl.children[2].textContent.replace(',', '.'));
        mountTypeSelect.value = getMountTypeValue(mountInfoEl.children[3].textContent);
        mountId = e.target.parentElement.dataset.mountId;

        createMountBtn.setAttribute('disabled', 'disabled');
        updateMountBtn.removeAttribute('disabled')
    }

    function getMountTypeValue(mountType) {
        let value;

        switch (mountType) {
            case 'Ground': value = 0; break;
            case 'Flying': value = 1; break;
            case 'Aquatic': value = 2; break;
        }

        return value;
    }

    async function deleteMount(e) {
        const mountId = e.target.parentElement.dataset.mountId;

        const response = await fetch(baseUrl + 'deletemount', {
            method: 'DELETE',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify({ id: mountId })
        });

        if (response.status == 200) {
            const cardsAndPaginationHolderView = await response.text();
            cardsAndPaginationHolder.innerHTML = cardsAndPaginationHolderView;
            ClearMountInputs();
        }
    }

    function handlePaginationClick(e) {
        if (e.target && e.target.classList.contains('pagination-button-js')) {
            switch (pageName) {
                case 'mount-admin': getMountPageResults(e);
            }
        }
    }

    async function getMountPageResults(e) {
        e.preventDefault();

        const pageNumber = e.target.classList.contains('pagination-button-js') ? e.target.dataset.value : document.querySelector('.active-pagination')?.dataset.value ?? 1;

        const response = await fetch(baseUrl + 'getpageresults' + `?pageNumber=${pageNumber}`, {
            method: 'GET',
            headers: {
                'content-type': 'application/json',
            },
        });

        if (response.status == 200) {
            const cardsAndPaginationHolderView = await response.text();
            cardsAndPaginationHolder.innerHTML = cardsAndPaginationHolderView;
        }
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

    function ClearMountInputs() {
        mountNameInput.value = '';
        mountDescriptionTextarea.value = '';
        mountRentPriceInput.value = '';
        mountTypeSelect.value = '';
        mountImageInput.value = '';
    }
}