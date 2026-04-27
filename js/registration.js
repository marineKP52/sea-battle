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
 
const errorMessages = {
    login: 'Логін має містити 6-12 символів (літери, цифри, _)',
    email: 'Введіть коректну електронну пошту',
    password: 'Пароль 6-12 символів: A-Z, a-z, 0-9, ?!#$%',
    'password-confirm': 'Паролі не співпадають'
};

submitBtn.disabled = true;
 
const updateButton = () => {
    const allValid = Array.from(formInputs).every(input => input.classList.contains('valid'));
    submitBtn.disabled = !allValid;
};
 
const showError = (input, message) => {
    let errorEl = input.closest('.modal__window__field').querySelector('.field-error');
    if (!errorEl) {
        errorEl = document.createElement('span');
        errorEl.classList.add('field-error');
        input.closest('.modal__window__field').appendChild(errorEl);
    }
    errorEl.textContent = message;
};
 
const hideError = (input) => {
    const errorEl = input.closest('.modal__window__field').querySelector('.field-error');
    if (errorEl) errorEl.textContent = '';
};
 
const validateInput = (input) => {
    const value = input.value.trim();
    let isValid = true;
 
    if (input.name === 'password-confirm') {
        isValid = value !== '' && value === pswInput.value.trim();
    } else {
        const regex = regexs[input.name];
        if (regex) isValid = regex.test(value);
    }
 
    if (!isValid) {
        input.classList.add('invalid');
        input.classList.remove('valid');
        showError(input, errorMessages[input.name] || 'Некоректне значення');
    } else {
        input.classList.remove('invalid');
        input.classList.add('valid');
        hideError(input);
    }
 
    updateButton();
    return isValid;
};
 
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
    input.addEventListener('blur', () => validateInput(input));
});
 
pswInput.addEventListener('input', () => {
    if (pswConfirmInput.classList.contains('valid') || pswConfirmInput.classList.contains('invalid')) {
        validateInput(pswConfirmInput);
    }
});
 
registrationForm.addEventListener('submit', (event) => {
    event.preventDefault();
 
    let allValid = true;
    formInputs.forEach((input) => {
        if (!validateInput(input)) allValid = false;
    });
 
    if (!allValid) return;
 
    const formData = {
        username: registrationForm.querySelector('input[name="login"]').value.trim(),
        email: registrationForm.querySelector('input[name="email"]').value.trim(),
        password: pswInput.value.trim()
    };
 
    console.log('[registration] Form data ready to submit:', formData);
});