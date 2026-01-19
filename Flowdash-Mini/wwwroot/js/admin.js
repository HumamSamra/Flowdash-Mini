// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
(() => {
    'use strict'

    // Fetch all the forms we want to apply custom Bootstrap validation styles to
    const forms = document.querySelectorAll('.needs-validation')

    // Loop over them and prevent submission
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault()
                event.stopPropagation()
            }

            form.classList.add('was-validated')
        }, false)
    })

    $('form.extra-validate').each(function () {
        const form = $(this);
        const submitBtn = form.find('button[type="submit"], input[type="submit"], button[data-action="submit"]');

        if (!form[0].checkValidity()) {
            submitBtn.addClass('disabled');
        }

        form.on('input change', 'input, select, textarea', () => {
            if (!form[0].checkValidity()) {
                submitBtn.addClass('disabled');
            } else {
                submitBtn.removeClass('disabled');
            }
        });
    });

    $('.refresh-btn').on('click', function () {
        window.location.reload()
    })

    // Data tables
    new DataTable('.data-table-list', {
        columnDefs: [
            {
                targets: '_all',        // or specific column index
                className: 'dt-left'    // forces header + cells to left-align
            }
        ]
    });


    $('#multiple-select-field').select2({
        theme: "bootstrap-5",
        width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
        placeholder: $(this).data('placeholder'),
        closeOnSelect: false,
    });
})()

$(document).on('click', 'tr.clickable-redirect > td:not(.avoid-click)', function (ev) {
    ev.preventDefault();
    ev.stopPropagation();
    window.location.href = $(this).closest("tr").attr("route");
});

$('input.daterange-picker').daterangepicker({
    locale: {
        format: 'DD/MM/YYYY',
        cancelLabel: 'Clear'
    },
});

$('input.daterange-picker').on('cancel.daterangepicker', function (ev, picker) {
    $(this).val('');
});

function copy(value) {
    if (!value) return;

    navigator.clipboard.writeText(value);

    const Toast = Swal.mixin({
        toast: true,
        position: "top-end",
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.onmouseenter = Swal.stopTimer;
            toast.onmouseleave = Swal.resumeTimer;
        }
    });
    Toast.fire({
        icon: "success",
        title: "Text copied successfully"
    });
}

function copyText(selector) {
    const text = document.querySelector(selector)?.innerText;
    if (!text) return;

    navigator.clipboard.writeText(text);

    const Toast = Swal.mixin({
        toast: true,
        position: "top-end",
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.onmouseenter = Swal.stopTimer;
            toast.onmouseleave = Swal.resumeTimer;
        }
    });
    Toast.fire({
        icon: "success",
        title: "Text copied successfully"
    });
}

function showLoader() {
    $("#loader-overlay").fadeIn(300); // 300ms fade in
}

function hideLoader() {
    $("#loader-overlay").fadeOut(300); // 300ms fade out
}