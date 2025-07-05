const enviroment = 'https://www.funsar.org.co';
//const enviroment = 'https://localhost:7029';

$(document).ready(function () {
    getStateBrigada();
    getCountData();
});

function getCountData() {
    const apiUrl = enviroment + '/Api/GetCountPerson';
    (async () => {
        try {
            const response = await fetch(apiUrl);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.json();
            console.log(result);

            const btn = document.getElementById('estudiantesContador');
            if (btn) {
                btn.innerText = result;
            } else {
                console.error('El elemento con id "estudiantesContador" no existe.');
            }
        } catch (error) {
            console.error('Failed to get status brigada:', error.message);
        }
    })();
}

function getStateBrigada() {
    const apiUrl = enviroment + '/Api/getStateBrigada';
    try
    {
        fetch(apiUrl)
            .then(async function (response) {
                const result = await response.json();
                var btn = document.getElementById('btnBrigada');
                if (result == 1) {
                    btn.innerText = 'Terminar Brigada';
                } else if (result == 2) {
                    btn.innerText = 'Iniciar Brigada';
                }
            })
            .catch(function (reason) {
                console.error('Failed to get status brigada', reason);
            });
    } catch (error) {
        console.error(error);
    }
}

var btnBrigada = document.getElementById('btnBrigada');

if (btnBrigada) {
    btnBrigada.addEventListener('click', async () => {
        var btn = document.getElementById('btnBrigada');
        if (btn.innerText == 'Iniciar Brigada') {
            // Mostrar SweetAlert para ingresar la fecha
            const { value: selectedDate } = await Swal.fire({
                title: 'Información',
                html: `
                <p>Por favor, selecciona la fecha de actualización:</p>
                <input type="date" id="input-date" class="swal2-input" placeholder="YYYY-MM-DD">
            `,
                showCancelButton: true,
                confirmButtonText: 'Confirmar',
                cancelButtonText: 'Cancelar',
                preConfirm: () => {
                    const dateInput = new Date(document.getElementById('input-date').value).toISOString();
                    if (!dateInput) {
                        Swal.showValidationMessage('Debes seleccionar una fecha válida.');
                    }
                    return dateInput;
                }
            });

            // Validar si se seleccionó una fecha
            if (selectedDate) {
                var apiUrl = enviroment + '/Api/SetStateBrigada?update=' + selectedDate; // Cambia por la URL completa si es necesario

                try {
                    const response = await fetch(apiUrl, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({ update: selectedDate })
                    });

                    if (response.ok) {
                        Swal.fire('Éxito', 'Brigada iniciada correctamente.', 'success').
                            then((result) => {
                                if (result.isConfirmed) {
                                    getStateBrigada();
                                }
                            });
                    } else {
                        Swal.fire('Error', 'Error al iniciar la brigada.', 'error');
                        console.error(`Error ${response.status}: ${response.statusText}`);
                    }
                } catch (error) {
                    Swal.fire('Error', 'Ocurrió un error al conectar con el servidor.', 'error');
                    console.error(error);
                }
            } else {
                Swal.fire('Cancelado', 'No se seleccionó una fecha.', 'info');
            }
        } else if (btn.innerText == 'Terminar Brigada') {
            Swal.fire({
                title: '¿Estas seguro de finalizar la brigada?',
                icon: 'info',
                showCancelButton: false,
                confirmButtonColor: '#DD6B55',
                confirmButtonText: 'Si, Continuar.'
            }).then(async (result) => {
                if (result.isConfirmed) {
                    const selectedDate = new Date().toISOString();
                    var apiUrl = enviroment + '/Api/SetStateBrigada?update=' + selectedDate; // Cambia por la URL completa si es necesario

                    try {
                        const response = await fetch(apiUrl, {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/json'
                            },
                            body: JSON.stringify({ update: selectedDate })
                        });

                        if (response.status === 204) {
                            Swal.fire('Información', 'Algunos participantes no han finalizado el proceso.', 'error').then((result) => {
                                if (result.isConfirmed) {
                                    getStateBrigada();
                                }
                            });
                        } else if (response.ok) {
                            Swal.fire('Éxito', 'Brigada finalizada correctamente.', 'success').then((result) => {
                                if (result.isConfirmed) {
                                    getStateBrigada();
                                }
                            });
                        } else {
                            Swal.fire('Error', 'Error al finalizar la brigada.', 'error');
                            console.error(`Error ${response.status}: ${response.statusText}`);
                        }
                    } catch (error) {
                        Swal.fire('Error', 'Ocurrió un error al conectar con el servidor.', 'error');
                        console.error(error);
                    }
                }
            });
        }

    });
}
