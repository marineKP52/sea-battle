const tableRows = document.querySelectorAll('.rating-block--transparent .rating-row');

async function loadRating() {
    const response = await fetch('...');
    const players = await response.json();

    tableRows.forEach((row, index) => {
        let player = players[index];

        if (!player) return;

        let username = row.querySelector('.rating-col--username');
        let score = row.querySelector('.rating-col--score');

        username.textContent = player.username;
        score.textContent = player.rating;

        if (index >= 3 && index <= 9) {
            let top = row.querySelector('.rating-circle');
            top.textContent = player.position;
        }

        if (player.id === 7) {
            row.classList.add('user');
        }
    });
}


loadRating();