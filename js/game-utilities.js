function createGameElement(tagName, classNames, parent) {
    let newElement = document.createElement(tagName);
    newElement.classList.add(...classNames.split(' '));
    parent.appendChild(newElement);
    return newElement;
}

const board = document.querySelector('.main__board-column__board');

for (let i = 0; i < 11; i++) {
    let cellsBox = createGameElement('div', 'main__board__box', board);
    for (let j = 0; j < 11; j++) {
        let cell = createGameElement('div', 'main__board__box__cell', cellsBox);
    }
}