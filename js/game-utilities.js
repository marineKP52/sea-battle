function createGameElement(tagName, classNames, parent) {
    let newElement = document.createElement(tagName);
    newElement.classList.add(...classNames.split(' '));
    parent.appendChild(newElement);
    return newElement;
}

const board = document.querySelector('.main__board');
const letters = "ABCDEFGHIJK";

for (let i = 0; i < 11; i++) {
    for (let j = 0; j < 11; j++) {
        let cell = createGameElement('div', 'main__board__cell', board);

        if (i === 0) cell.style.borderTop = "none";
        if (j === 10) cell.style.borderRight = "none";

        if (i === 0) {
            cell.textContent = letters[j - 1];
            cell.classList.add("label");
        }
        else if (j === 0) {
            cell.textContent = i;
            cell.classList.add("label");
        }
    }
}