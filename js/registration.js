
const registrationForm = document.getElementById('registration-form');
const formInputs = registrationForm.querySelectorAll('.modal__window__field input');
const eyeImgs = document.querySelectorAll('.modal__window__field__eye');
const pswInput = document.querySelector('input[name="password"]');
const pswConfirmInput = document.querySelector('input[name="password-confirm"]');
const submitBtn = document.getElementById('register');

const regexs = {
    login: /^[a-zA-Z0-9_]{6,12}$/,
    email: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
    password: /^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[?!#$%])[a-zA-Z0-9_?!#$%]{6,12}$/
};

function validateInput(input) {
    let value = input.value.trim();

    if (input.name === 'password-confirm') {
        let isValid = value !== '' && value === pswInput.value.trim();
        isValid ? hideError(input) : showError(input);
        return isValid;
    }

    let regex = regexs[input.name];
    if (!regex) return true;

    let isValid = regex.test(value);
    isValid ? hideError(input) : showError(input);
    return isValid;
}

function showError(input, text = null) {
    let message = input.nextElementSibling;
    input.classList.add('invalid');
    input.classList.remove('valid');
    if (text !== null) {
        message.textContent = text;
    }
    message.classList.remove('hidden');
}

function hideError(input) {
    let message = input.nextElementSibling;
    input.classList.remove('invalid');
    input.classList.add('valid');
    message.classList.add('hidden');
}

function changeInputLabel(fieldName, form, content) {
    let errorInput = form.querySelector(`input[name="${fieldName}"]`);
    if (!errorInput) return;
    showError(errorInput, content);
}

function updateButton() {
    const allValid = Array.from(formInputs).every(input => input.classList.contains('valid'));
    submitBtn.disabled = !allValid;
}

submitBtn.disabled = true;

const originalHideError = hideError;
const originalShowError = showError;



async function userRegister(userData) {
    let response = await fetch('http://localhost:5231/api/auth/register', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        },
        credentials: 'include',
        body: JSON.stringify(userData)
    });

    let result = await response.json();
    return result;
}

async function registrationProcess(formData) {
    let result = await userRegister(formData);

    if (result.status === 0) {
        window.location.replace('../pages/rules.html');
    } else if (result.status === 1) {
        alert(result.data);
    } else if (result.status === 2) {
        changeInputLabel(result.field, registrationForm, result.data);
    }
}

eyeImgs.forEach((eyeImg) => {
    eyeImg.addEventListener('click', () => {
        const input = eyeImg.closest('.modal__window__field').querySelector('input');
        if (input.type === 'password') {
            input.type = 'text';
            eyeImg.src = '../img/hide.png';
        } else {
            input.type = 'password';
            eyeImg.src = '../img/view.png';
        }
    });
});

formInputs.forEach((input) => {
    input.addEventListener('blur', () => {
        validateInput(input);
        updateButton();
    });
});

pswInput.addEventListener('input', () => {
    if (pswConfirmInput.classList.contains('valid') ||
        pswConfirmInput.classList.contains('invalid')
    ) {
        validateInput(pswConfirmInput);
        updateButton();
    }
});

registrationForm.addEventListener('submit', async (event) => {
    event.preventDefault();

    let isValid = true;
    formInputs.forEach((input) => {
        if (!validateInput(input)) isValid = false;
    });
    updateButton();

    if (!isValid) return;

    const formData = {
        username: registrationForm.querySelector('input[name="login"]').value.trim(),
        email: registrationForm.querySelector('input[name="email"]').value.trim(),
        password: pswInput.value.trim()
    };

    await registrationProcess(formData);
});
