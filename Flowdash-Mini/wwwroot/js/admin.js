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

        form.on('input', () => {
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


    tinymce.init({
        selector: 'textarea.tinymce',
        height: 300,
        plugins: 'preview importcss searchreplace autolink autosave save directionality code visualblocks visualchars fullscreen image link media codesample table charmap pagebreak nonbreaking anchor insertdatetime advlist lists wordcount help charmap quickbars emoticons accordion',
        menubar: 'file edit view insert format tools table help',
        toolbar: "undo redo | blocks fontfamily fontsize | accordion accordionremove | bold italic underline strikethrough | align numlist bullist | link image | table media | lineheight outdent indent| forecolor backcolor removeformat | charmap emoticons | code fullscreen preview | save print | pagebreak anchor codesample | ltr rtl",
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
