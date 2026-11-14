document.addEventListener("DOMContentLoaded", function () {
    inicializarSistemaSeleccion('autores');
    inicializarSistemaSeleccion('categorias');
});

function inicializarSistemaSeleccion(tipo) {
    const inputBuscar = document.getElementById(`buscador${capitalizar(tipo)}`);
    const contenedorResultados = document.getElementById(`resultados${capitalizar(tipo)}`);
    const contenedorSeleccionados = document.getElementById(`contenedor${capitalizar(tipo)}Seleccionados`);
    const nombreInputHidden = tipo === 'autores' ? 'IdsAutores' : 'IdsCategorias';
    const listaDisponibles = window[`lista${capitalizar(tipo)}Disponibles`] || [];

    if (!inputBuscar) return;

    inputBuscar.addEventListener('input', function () {
        const texto = this.value.toLowerCase().trim();
        contenedorResultados.innerHTML = '';

        if (texto.length === 0) {
            contenedorResultados.style.display = 'none';
            return;
        }

        const filtrados = listaDisponibles.filter(item => {
            const nombre = (item.Nombre ? `${item.Nombre} ${item.Apellido}` : item.Descripcion).toLowerCase();
            const yaSeleccionado = document.querySelector(`input[name="${nombreInputHidden}"][value="${item.Id}"]`);
            return nombre.includes(texto) && !yaSeleccionado;
        });

        if (filtrados.length > 0) {
            contenedorResultados.style.display = 'block';
            filtrados.forEach(item => {
                const div = document.createElement('div');
                div.className = 'list-group-item list-group-item-action cursor-pointer';
                div.textContent = item.Nombre ? `${item.Nombre} ${item.Apellido}` : item.Descripcion;

                div.addEventListener('click', function () {
                    agregarSeleccionado(item, tipo, nombreInputHidden, contenedorSeleccionados);
                    inputBuscar.value = '';
                    contenedorResultados.innerHTML = '';
                    contenedorResultados.style.display = 'none';
                    sincronizarCheckboxModal(tipo, item.Id, true);
                });

                contenedorResultados.appendChild(div);
            });
        } else {
            contenedorResultados.style.display = 'none';
        }
    });

    document.addEventListener('click', function (e) {
        if (!inputBuscar.contains(e.target) && !contenedorResultados.contains(e.target)) {
            contenedorResultados.style.display = 'none';
        }
    });

    const checkboxesModal = document.querySelectorAll(`.chk-modal-${tipo}`);
    checkboxesModal.forEach(chk => {
        chk.addEventListener('change', function () {
            const id = parseInt(this.value);
            const item = listaDisponibles.find(i => i.Id === id);
            if (this.checked) {
                if (!document.querySelector(`input[name="${nombreInputHidden}"][value="${id}"]`)) {
                    agregarSeleccionado(item, tipo, nombreInputHidden, contenedorSeleccionados);
                }
            } else {
                removerSeleccionado(id, tipo, nombreInputHidden);
            }
        });
    });
}

function agregarSeleccionado(item, tipo, nombreInputHidden, contenedorSeleccionados) {
    const id = item.Id;
    const texto = item.Nombre ? `${item.Nombre} ${item.Apellido}` : item.Descripcion;

    const badge = document.createElement('span');
    badge.className = 'badge bg-primary d-inline-flex align-items-center gap-1 p-2 fs-6 m-1';
    badge.setAttribute('data-id', id);
    badge.textContent = texto;

    const btnCerrar = document.createElement('button');
    btnCerrar.type = 'button';
    btnCerrar.className = 'btn-close btn-close-white btn-sm';
    btnCerrar.setAttribute('aria-label', `Eliminar ${texto}`);

    btnCerrar.addEventListener('click', function () {
        badge.remove();
        const inputHidden = document.getElementById(`hidden_${tipo}_${id}`);
        if (inputHidden) inputHidden.remove();
        sincronizarCheckboxModal(tipo, id, false);
    });

    badge.appendChild(btnCerrar);
    contenedorSeleccionados.appendChild(badge);

    const inputHidden = document.createElement('input');
    inputHidden.type = 'hidden';
    inputHidden.name = nombreInputHidden;
    inputHidden.value = id;
    inputHidden.id = `hidden_${tipo}_${id}`;

    document.forms[0].appendChild(inputHidden);
}

function removerSeleccionado(id, tipo, nombreInputHidden) {
    const badge = document.querySelector(`#contenedor${capitalizar(tipo)}Seleccionados [data-id="${id}"]`);
    if (badge) badge.remove();
    const inputHidden = document.getElementById(`hidden_${tipo}_${id}`);
    if (inputHidden) inputHidden.remove();
}

function sincronizarCheckboxModal(tipo, id, estado) {
    const chk = document.getElementById(`chk_modal_${tipo}_${id}`);
    if (chk) chk.checked = estado;
}

function capitalizar(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
}