document.addEventListener("DOMContentLoaded", function () {
    configurarFiltroSelect("txtBuscarSocio", "idSocio", "estadoBusquedaSocio");
    configurarFiltroSelect("txtBuscarEjemplar", "idEjemplar", "estadoBusquedaEjemplar");
    configurarResumenSocio("idSocio");
});

function configurarFiltroSelect(idInput, idSelect, idEstado) {
    const input = document.getElementById(idInput);
    const select = document.getElementById(idSelect);
    const estado = document.getElementById(idEstado);

    if (!input || !select) return;

    const opcionesOriginales = Array.from(select.options).map(opt => ({
        value: opt.value,
        text: opt.text
    }));

    input.addEventListener("input", function () {
        const termino = this.value.toLowerCase().trim();
        const valorActual = select.value;

        select.innerHTML = "";

        const coincidencias = opcionesOriginales.filter((opt, index) => {
            if (index === 0 && opt.value === "") return true;
            return opt.text.toLowerCase().includes(termino);
        });

        coincidencias.forEach(opt => {
            const optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.text = opt.text;
            select.appendChild(optionElement);
        });

        const opcionesValidas = coincidencias.filter(o => o.value !== "");

        if (termino.length > 0 && opcionesValidas.length > 0) {
            select.value = opcionesValidas[0].value;
            select.dispatchEvent(new Event("change"));
            if (estado) {
                estado.textContent = opcionesValidas.length === 1
                    ? "1 coincidencia seleccionada automáticamente: " + opcionesValidas[0].text
                    : opcionesValidas.length + " coincidencias encontradas. Seleccionada: " + opcionesValidas[0].text;
            }
        } else if (termino.length > 0 && opcionesValidas.length === 0) {
            select.value = "";
            select.dispatchEvent(new Event("change"));
            if (estado) {
                estado.textContent = "No se encontraron coincidencias.";
            }
        } else {
            select.value = valorActual;
            select.dispatchEvent(new Event("change"));
            if (estado) {
                estado.textContent = "";
            }
        }
    });
}

function configurarResumenSocio(idSelectSocio) {
    const selectSocio = document.getElementById(idSelectSocio);
    const panelResumen = document.getElementById("resumenSocioPanel");

    if (!selectSocio || !panelResumen) return;

    const socios = window.sociosHabilitadosData || [];

    function actualizarPanel() {
        const idSeleccionado = parseInt(selectSocio.value, 10);
        const socio = socios.find(s => s.Id === idSeleccionado);

        if (!socio) {
            panelResumen.classList.add("d-none");
            return;
        }

        const descripcionPolitica = socio.Politica || "Sin política";
        const maxPrestamos = socio.MaxPrestamos != null ? socio.MaxPrestamos : 0;
        const activos = socio.PrestamosActivos != null ? socio.PrestamosActivos : 0;
        const diasDuracion = socio.DiasDuracion != null ? socio.DiasDuracion : 0;

        const fechaInicio = new Date();
        const fechaDevolucion = new Date();
        fechaDevolucion.setDate(fechaInicio.getDate() + diasDuracion);

        const lblActivos = document.getElementById("lblActivosSocio");
        lblActivos.textContent = `${activos} de ${maxPrestamos} permitidos`;
        lblActivos.className = activos >= maxPrestamos ? "fw-bold text-danger" : "fw-semibold text-success";

        document.getElementById("lblPoliticaSocio").textContent = descripcionPolitica;
        document.getElementById("lblFechaInicioPrestamo").textContent = formatearFecha(fechaInicio);
        document.getElementById("lblFechaDevolucionPrestamo").textContent = formatearFecha(fechaDevolucion) + " (" + diasDuracion + " días)";

        panelResumen.classList.remove("d-none");
    }

    selectSocio.addEventListener("change", actualizarPanel);
    actualizarPanel();
}

function formatearFecha(fecha) {
    const dia = String(fecha.getDate()).padStart(2, "0");
    const mes = String(fecha.getMonth() + 1).padStart(2, "0");
    const anio = fecha.getFullYear();
    return `${dia}/${mes}/${anio}`;
}