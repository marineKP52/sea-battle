const BOARD_SIZE = 10;

let boardState = [];

for (let i = 0; i < BOARD_SIZE; i++) {
    let row = [];

    for (let j = 0; j < BOARD_SIZE; j++) {
        row.push(0);
    }

    boardState.push(row);
}

let board = document.querySelector(".main__board");

board.addEventListener("click", function(event) {

    let cell = event.target;

    if (!cell.classList.contains("main__board__cell")) {
        return;
    }

    let x = parseInt(cell.dataset.x);
    let y = parseInt(cell.dataset.y);

    if (boardState[x][y] === 0) {
        boardState[x][y] = 1;
        cell.classList.add("filled");
    }
    else {
        boardState[x][y] = 0;
        cell.classList.remove("filled");
    }

});


function getShips(board) {
    
    let visited = [];

    for (let i = 0; i < BOARD_SIZE; i++) {
        visited.push([]);

        for (let j = 0; j < BOARD_SIZE; j++) {
            visited[i].push(false);
        }
    }

    let ships = [];

    for (let i = 0; i < BOARD_SIZE; i++) {
        for (let j = 0; j < BOARD_SIZE; j++) {
            if (board[i][j] === 1 && visited[i][j] === false) {
                let cells = explore(board, visited, i, j);
                ships.push(cells);
            }
        }
    }

    return ships;
}


function explore(board, visited, x, y) {
    let stack = [];
    stack.push({ x: x, y: y });

    let cells = [];

    while (stack.length > 0) {
        let cell = stack.pop();

        let cellX = cell.x;
        let cellY = cell.y;

        if (cellX < 0 || cellY < 0 
            || cellX >= BOARD_SIZE || cellY >= BOARD_SIZE
        )
        {
            continue;
        }

        if (visited[cellX][cellY] === true) {
            continue;
        }

        if (board[cellX][cellY] === 0) {
            continue;
        }

        visited[cellX][cellY] = true;
        cells.push({ x: cellX, y: cellY });

        stack.push({ x: cellX + 1, y: cellY });
        stack.push({ x: cellX - 1, y: cellY });
        stack.push({ x: cellX, y: cellY + 1 });
        stack.push({ x: cellX, y: cellY - 1 });
    }

    return cells;
}

function checkShipsCount(ships) {
    let expected = {
        1: 4,
        2: 3,
        3: 2, 
        4: 1
    };

    let real = {};

    for (let i = 0; i < ships.length; i++) {
        let size = ships[i].length;

        if (real[size] === undefined) {
            real[size] = 0;
        }

        real[size] = real[size] + 1;
    }

    for (var key in expected) {
        if(real[key] !== expected[key]) {
            return false;
        }
    }

    return true;
}

function checkNoDiagonalTouch(board) {
    for (let i = 0; i < BOARD_SIZE; i++) {
        for (let j = 0; j < BOARD_SIZE; j++) {
            if (board[i][j] === 1) {
                for (let dx = -1; dx <= 1; dx++) {
                    for (let dy = -1; dy <= 1; dy++) {
                        let x = i + dx;
                        let y = j + dy;

                        if (x >= 0 && y >= 0 &&
                            x < BOARD_SIZE && y < BOARD_SIZE
                        )
                        {
                            if (board[x][y] === 1) {
                                if (!(x === i && y === j)) {
                                    let dxAbs = Math.abs(x-i);
                                    let dyAbs = Math.abs(y-j);

                                    if (dxAbs === 1 && dyAbs === 1) {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    return true;
}


function checkShipsShape(board) {
    let ships = getShips(board);

    for (let i = 0; i < ships.length; i++) {
        if (!isStraightLine(ships[i])) {
            return false;
        }
    }

    return true;
}


function isStraightLine(ship) {
    if (ship.length === 1) {
        return true;
    }

    let sameX = true;
    let sameY = true;

    for (let i = 1; i < ship.length; i++) {

        if (ship[i].x !== ship[0].x) {
            sameX = false;
        }

        if (ship[i].y !== ship[0].y) {
            sameY = false;
        }
    }

    if (!(sameX || sameY)) {
        return false;
    }

    if (sameX) {
        let ys = ship.map(c => c.y).sort((a, b) => a - b);

        for (let i = 1; i < ys.length; i++) {
            if (ys[i] !== ys[i - 1] + 1) {
                return false;
            }
        }
    }

    if (sameY) {
        let xs = ship.map(c => c.x).sort((a, b) => a - b);

        for (let i = 1; i < xs.length; i++) {
            if (xs[i] !== xs[i - 1] + 1) {
                return false;
            }
        }
    }

    return true;
}

function validateBoard(board) {

    let ships = getShips(board);

    if (!checkShipsCount(ships)) {
        return false;
    }

    if (!checkNoDiagonalTouch(board)) {
        return false;
    }

    if (!checkShipsShape(board)) {
         return false;
    }

    return true;
}

let playButton = document.getElementById("play");

playButton.addEventListener("click", function () {

    if (!validateBoard(boardState)) {
        alert("Неправильна розстановка кораблів!");
        return;
    }

    fetch("...", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            board: boardState
        })
    });

});