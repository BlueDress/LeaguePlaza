document.addEventListener("DOMContentLoaded", navigationMain());

function navigationMain() {
    const hamburgerMenu = document.querySelector('#hamburger-menu');

    hamburgerMenu.addEventListener('click', e => handleHamburgerMenuClick(e));

    function handleHamburgerMenuClick(e) {
        const menuOpen = document.querySelector('#menu-open');
        const menuClose = document.querySelector('#menu-close');
        const navLinks = document.querySelector('#nav-links');

        if (e.target.id === 'menu-open') {
            menuOpen.classList.add('display-none');
            menuClose.classList.remove('display-none');
            navLinks.classList.remove('display-none');
        }

        if (e.target.id === 'menu-close') {
            menuOpen.classList.remove('display-none');
            menuClose.classList.add('display-none');
            navLinks.classList.add('display-none');
        }
    }
}