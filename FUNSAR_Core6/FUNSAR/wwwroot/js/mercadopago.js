const enviroment = 'https://www.funsar.org.co';
//const enviroment = 'https://localhost:7029';
var service;
var selectedFinancialInstitutionId;
var selectedTypePerson;
var selectedTypeDocument;

document.getElementById('form-checkout__personType').addEventListener('change', e => {
    const personTypesElement = document.getElementById('form-checkout__personType');
    updateSelectOptions(personTypesElement.value);
});

document.getElementById('paymentForm').addEventListener('submit', function (event) {
    event.preventDefault();

    const nombreR = document.getElementById('nombreR').value.trim();
    const apellidoR = document.getElementById('apellidoR').value.trim();
    const tipoPersona = document.getElementById('form-checkout__personType').value.trim();
    const documentoR = document.getElementById('documentoR').value.trim();
    const correoR = document.getElementById('correoRInput').value.trim();
    const celularR = document.getElementById('celularR').value.trim();
    const direccionR = document.getElementById('direccionR').value.trim();
    const tipoDocumento = document.getElementById('form-checkout__identificationType').value.trim();

    const banco = getCookie('selectedFinancialInstitutionId');
    const documentoP = getCookie('IdVoluntario');
    const id = service;

    function validateEmail(email) {
        const re = /^[^\s@]+@[^\s@]+\.(com|org|net|edu|gov|co|io|es)$/i;
        return re.test(email);
    }

    if (!nombreR || !apellidoR || !tipoPersona || !documentoR || !correoR || !celularR || !tipoDocumento || !direccionR) {
        document.getElementById('lblErrorValidate').textContent = 'Todos los campos son obligatorios.';
        return;
    }

    if (!validateEmail(correoR)) {
        document.getElementById('lblErrorValidate').textContent = 'El formato del correo electrónico es inválido.';
        return;
    }

    if (!banco) {
        document.getElementById('lblErrorValidate').textContent = 'Ocurrio un error obteniendo los datos del banco.';
        return;
    }

    const formData = {
        id,
        nombreR,
        apellidoR,
        tipoPersona,
        tipoDocumento,
        documentoR,
        correoR,
        celularR,
        direccionR,
        banco,
        documentoP
    };

    const jsonData = JSON.stringify(formData);

    fetch(enviroment+'/Api/pagoServicios', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-CSRF-TOKEN': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: jsonData
    })
        .then(response => response.json())
        .then(data => {
            try {
                console.log(data);
                if (data) {
                    const jsonObject = JSON.parse(data);
                    window.location.href = jsonObject.transaction_details.external_resource_url;
                } else {
                    document.getElementById('lblErrorValidate').textContent = 'Error en el procesamiento del pago.';
                }
            } catch (err) {
                document.getElementById('lblErrorValidate').textContent = 'La respuesta del servidor no es válida.';
                console.error('Error parsing JSON:', err);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            document.getElementById('lblErrorValidate').textContent = 'Error en la comunicación con el servidor.';
        });
});

function updateSelectOptions(selectedValue) {
    const naturalDocTypes = [
        new Option('CC', 'CC'),
        new Option('CE.', 'CE'),
        new Option('Otro', 'Otro')
    ];
    const juridicaDocTypes = [
        new Option('NIT', 'NIT')
    ];
    const idDocTypes = document.getElementById('form-checkout__identificationType');

    if (selectedValue === 'natural') {
        idDocTypes.options.length = 0;
        naturalDocTypes.forEach(item => idDocTypes.options.add(item, undefined));
    } else {
        idDocTypes.options.length = 0;
        juridicaDocTypes.forEach(item => idDocTypes.options.add(item, undefined));
    }

    var currentDate = new Date();
    var expirationTime = new Date(currentDate.getTime() + 600 * 1000);
    var expires = expirationTime.toUTCString();

    const personTypesElement = document.getElementById('form-checkout__personType');
    const personTypeValue = personTypesElement.value;
    document.cookie = 'personType=' + personTypeValue + '; expires=' + expires + '; path=/';

    const identificationTypeElement = document.getElementById('form-checkout__identificationType');
    const identificationTypeValue = identificationTypeElement.value;
    document.cookie = 'identificationType=' + identificationTypeValue + '; expires=' + expires + '; path=/';
}

function setPse() {
    fetch(enviroment + '/Api/ListarBancos')
        .then(async function (response) {
            const paymentMethods = await response.json();

            if (paymentMethods && paymentMethods.length > 0) {
                const pse = paymentMethods[0];

                if (pse.financialInstitutions && pse.financialInstitutions.length > 0) {
                    const banksList = pse.financialInstitutions;
                    const banksListElement = document.getElementById('banksList');
                    const selectElement = document.createElement('select');
                    selectElement.className = 'form-control';
                    selectElement.name = 'financialInstitution';

                    const defaultOption = document.createElement('option');
                    defaultOption.value = '';
                    defaultOption.textContent = '--Seleccione--';
                    selectElement.appendChild(defaultOption);

                    banksList.forEach(bank => {
                        const option = document.createElement('option');
                        option.value = bank.id;
                        option.textContent = bank.description;
                        selectElement.appendChild(option);
                    });

                    selectElement.addEventListener('change', function () {
                        const selectedFinancialInstitutionId = selectElement.value;
                        console.log('Valor seleccionado:', selectedFinancialInstitutionId);

                        var currentDate = new Date();
                        var expirationTime = new Date(currentDate.getTime() + 600 * 1000);
                        var expires = expirationTime.toUTCString();
                        document.cookie = 'selectedFinancialInstitutionId=' + selectedFinancialInstitutionId + '; expires=' + expires + '; path=/';
                    });

                    banksListElement.appendChild(selectElement);
                } else {
                    console.error('Financial institutions not found or empty.');
                }
            } else {
                console.error('Payment methods not found or empty.');
            }
        })
        .catch(function (reason) {
            console.error('Failed to get payment methods', reason);
        });
}


function showDataPayment() {
    fetch(enviroment+'/Api/DatosPago')
        .then(async function (response) {
            const responseData = await response.json();

            if (responseData.data != null) {
                service = responseData.data.id;
                if (responseData.data.detalle != null) {
                    var labelDetail = document.getElementById('detail');
                    var labelValue = document.getElementById('value');

                    var currentDate = new Date();
                    var expirationTime = new Date(currentDate.getTime() + 600 * 1000);
                    var expires = expirationTime.toUTCString();
                    document.cookie = 'servicioValor=' + responseData.data.valor + '; expires=' + expires + '; path=/';

                    const formattedValue = parseFloat(responseData.data.valor).toLocaleString('es-ES', {
                        style: 'currency',
                        currency: 'COP'
                    });
                    labelDetail.innerHTML = '<h5><strong>Concepto pago: </strong></h5>' + responseData.data.detalle;
                    labelValue.innerHTML = '<h5><strong>Total a pagar: </strong></h5>' + '$' + formattedValue;
                } else {
                    document.getElementById('lblErrorValidate').textContent = 'Ocurrio un error obteniendo los datos de pago. ID:1';
                    console.error('Financial institutions not found or empty.');
                    return;
                }
            } else {
                document.getElementById('lblErrorValidate').textContent = 'Ocurrio un error obteniendo los datos de pago. ID: 2';
                console.error('Payment methods not found or empty.');
                return;
            }
        })
        .catch(function (reason) {
            document.getElementById('lblErrorValidate').textContent = 'Ocurrio un error obteniendo los datos de pago. ID: 3';
            console.error('Failed to get payment methods', reason);
            return;
        });
}

(function initCheckout() {
    try {
        setPse();
        showDataPayment();
        updateSelectOptions('natural')
    } catch (e) {
        return console.error('Error getting identificationTypes: ', e);
    }
})();

const mp = new MercadoPago(window.BearerToken);
const bricksBuilder = mp.bricks();

mp.bricks().create("wallet", "wallet_container", {
    initialization: {
        preferenceId: "<PREFERENCE_ID>",
    },
});

function getCookie(name) {
    let nameEQ = name + "=";
    let cookiesArray = document.cookie.split(';');

    for (let i = 0; i < cookiesArray.length; i++) {
        let cookie = cookiesArray[i].trim();
        if (cookie.indexOf(nameEQ) == 0) {
            return cookie.substring(nameEQ.length, cookie.length);
        }
    }

    return null;
}

