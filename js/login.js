const loginForm = document.getElementById('login-form');
const formInputs = loginForm.querySelectorAll('.modal__window__field input');

const eyeImg = document.querySelector('.modal__window__field__eye');
const pswInput = document.querySelector('input[name="password"]');

let regexs = {
    login: /^[a-zA-Z0-9_]{6,12}$/,
    password: /^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[?!#$%])[a-zA-Z0-9_?!#$%]{6,12}$/
};

function validateInput(input) {
    let value = input.value.trim();
    let regex = regexs[input.name];
    let message = input.nextElementSibling;

    if (!regex.test(value)) {
        input.classList.add('invalid');
        message.classList.remove('hidden');
        input.classList.remove('valid');
        return false;
    }
    else {
        input.classList.remove('invalid');
        input.classList.add('valid');
        message.classList.add('hidden');
        return true;
    }
}

function changeInputLabel(inputName, form, content) {
    let errorInput = form.querySelector(`input[name="${inputName}"]`);
    
    errorInput.classList.add('invalid');
    errorInput.classList.remove('valid');

    let message = errorInput.nextElementSibling;
    message.textContent = content;
    message.classList.remove('hidden');
}

async function userAuth(userData) {
    let response = await fetch('/', { 
        method: "POST", 
        headers: { 
            "Content-Type": "application/json", 
            'X-Requested-With': 'XMLHttpRequest' 
        }, 
        body: JSON.stringify(userData) 
    }); 
    
    let result = await response.json(); 
    return result;
}

async function loginProcess(log, psw) {
    let userData = {
        login: log,
        password: psw
    };

    let result = await userAuth(userData);
    if (result.status === 0) window.location.replace("../pages/rules.html");
    else if (result.status === 1) {
        alert(result.data);
    }
    else {
        changeInputLabel(result.field, loginForm, result.data);
    }
}


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
        validateInput(input);
    });
});


loginForm.addEventListener('submit', async (event) => {
    
    event.preventDefault();

    let isValid = true;

    formInputs.forEach((input) => {
        if (!validateInput(input)) {
            isValid = false;
        }
    });

    if (isValid) {
        let log = loginForm.elements.login.value.trim();
        let psw = loginForm.elements.password.value.trim();
        await loginProcess(log, psw);
    }
});