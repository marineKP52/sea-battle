const tableRows = document.querySelectorAll('.rating-block--transparent .rating-row');

const players = [
    { username: "Player1", score: 14000 },
    { username: "Player2", score: 13000 },
    { username: "Player3", score: 12000 },
    { username: "Player4", score: 11000 },
    { username: "Player5", score: 10000 },
    { username: "Player6", score: 9900 },
    { username: "Player7", score: 9700 },
    { username: "Player8", score: 9300 },
    { username: "Player9", score: 8800 },
    { username: "Player10", score: 7300 },
    { username: "Player18", score: 2500 }
];

tableRows.forEach((row, index) => {

    let player = players[index];

    let username = row.querySelector('.rating-col--username');
    let score = row.querySelector('.rating-col--score');

    username.textContent = player.username;
    score.textContent = player.score;

    if (index >= 3 && index <= 9) {
        let top = row.querySelector('.rating-circle');
        top.textContent = index + 1;
    }

    if (index === 4) {
        row.classList.add('user');
    }
});