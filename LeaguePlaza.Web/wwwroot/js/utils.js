function createElement(tag, text, attributes) {
    const element = document.createElement(tag);
    element.innerText = text;

    for (const [key, value] of Object.entries(attributes)) {
        element.setAttribute(key, value);
    }

    return element;
}