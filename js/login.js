const loginForm = document.getElementById('login-form');
const formInputs = loginForm.querySelectorAll('.modal__window__field input');

const eyeImg = document.querySelector('.modal__window__field__eye');
const pswInput = document.querySelector('input[name="password"');

let regexs = {
    login: /^[a-zA-Z0-9_]{6,12}$/,
    password: /^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[?!#$%])[a-zA-Z0-9_?!#$%]{6,12}$/
};

eyeImg.addEventListener('click', () => {
    
    if (pswInput.type === 'password') {
        pswInput.type = 'text';
        eyeImg.src = '../img/hide.png';
    }
    else {
        pswInput.type = 'password';
        eyeImg.src = '../img/view.png';
    }

});


formInputs.forEach((input) => {
    input.addEventListener('blur', () => {
        
        let value = input.value.trim();
        let regex = regexs[input.name];

        if (regex && !regex.test(value)) {
            input.classList.add('invalid');
        }
        else {
            input.classList.remove('invalid');
            input.classList.add('valid');
        }

    });
});


loginForm.addEventListener('submit', (event) => {
    
    event.preventDefault();



});