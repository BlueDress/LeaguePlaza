document.addEventListener("DOMContentLoaded", navigationMain());

function navigationMain() {
    const baseOrderUrl = '/api/orderapi/';

    const hamburgerMenu = document.querySelector('#hamburger-menu');
    const cartCountBadge = document.querySelector('.cart-nav-icon > span');

    const navSubMenuArrows = document.querySelectorAll('.nav-sub-menu-arrow-js');

    hamburgerMenu.addEventListener('click', e => handleHamburgerMenuClick(e));
    navSubMenuArrows.forEach(a => a.addEventListener('click', e => handleNavSubMenuArrowClick(e)));

    if (cartCountBadge) {
        updateCartCountBadge();
    }

    function handleHamburgerMenuClick(e) {
        const menuOpen = document.querySelector('#menu-open');
        const menuClose = document.querySelector('#menu-close');
        const navLinks = document.querySelector('#nav-links');

        if (e.target.id === 'menu-open') {
            menuOpen.classList.add('display-none');
            menuClose.classList.remove('display-none');
            navLinks.classList.add('open');
        }

        if (e.target.id === 'menu-close') {
            menuOpen.classList.remove('display-none');
            menuClose.classList.add('display-none');
            navLinks.classList.remove('open');
        }
    }

    async function updateCartCountBadge() {
        const response = await fetch(baseOrderUrl + 'getcartitemscount');

        if (response.status == 200) {
            cartCountBadge.textContent = await response.text();
        }
    }

    function handleNavSubMenuArrowClick(e) {
        e.preventDefault();

        const navSubLinks = e.target.closest('li').querySelector('.nav-sub-links-js');
        const arrows = e.target.closest('a').querySelectorAll('.nav-sub-menu-arrow-js');

        if (navSubLinks.classList.contains('open')) {
            navSubLinks.classList.remove('open');
        } else {
            document.querySelectorAll('.nav-sub-links-js').forEach(el => el.classList.remove('open'));
            document.querySelectorAll('.fa-chevron-down').forEach(el => el.parentElement.classList.remove('display-none'));
            document.querySelectorAll('.fa-chevron-up').forEach(el => el.parentElement.classList.add('display-none'));
            navSubLinks.classList.add('open');
        }

        arrows.forEach(a => a.classList.toggle('display-none'));
    }
}