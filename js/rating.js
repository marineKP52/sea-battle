let tableRows = document.querySelectorAll('.rating-block--transparent .rating-row');
let currentUser = JSON.parse(localStorage.getItem('userData'));

async function loadRating() {
    let response = await fetch('/api/rating', 
    {
        method: 'POST',
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            id: currentUser.id
        })
    });     
    let players = await response.json();

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

        if (player.id === currentUser.id) {
            row.classList.add('user');
        }
    });
}


loadRating();