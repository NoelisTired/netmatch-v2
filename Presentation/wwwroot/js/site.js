// TourWeb Portal: centrale UI-initialisatie.
// Alle bibliotheken laden met defer; dit script draait op DOMContentLoaded
// zodat lucide, GLightbox, Tom Select, flatpickr en List.js beschikbaar zijn.
(function () {
    'use strict';

    var TW = window.TW = window.TW || {};

    /* ---------------- Iconen (lucide) ---------------- */
    TW.icons = function () {
        if (window.lucide) {
            window.lucide.createIcons({ attrs: { 'stroke-width': 1.75 } });
        }
    };

    /* ---------------- Thema ---------------- */
    function initTheme() {
        var toggle = document.getElementById('theme-toggle');
        if (!toggle) return;
        toggle.addEventListener('click', function () {
            var isDark = document.documentElement.classList.toggle('dark');
            try { localStorage.setItem('tw-theme', isDark ? 'dark' : 'light'); } catch (e) { /* prima */ }
            window.dispatchEvent(new CustomEvent('tw:theme', { detail: { dark: isDark } }));
        });
    }

    /* ---------------- Lichtbak voor afbeeldingen ---------------- */
    function initLightbox() {
        if (window.GLightbox && document.querySelector('.glightbox')) {
            window.GLightbox({ selector: '.glightbox' });
        }
    }

    /* ---------------- Nette dropdowns (Tom Select) ---------------- */
    function initSelects() {
        if (!window.TomSelect) return;
        document.querySelectorAll('select[data-ts]').forEach(function (el) {
            if (el.tomselect) return;
            el.classList.remove('field');
            new window.TomSelect(el, {
                create: false,
                allowEmptyOption: true,
                controlInput: null,
                maxOptions: null
            });
        });
    }

    /* ---------------- Datumkiezer (flatpickr) ---------------- */
    function initDatePickers() {
        if (!window.flatpickr) return;
        var lang = document.documentElement.lang || 'nl';
        var locale = window.flatpickr.l10ns && window.flatpickr.l10ns[lang];
        document.querySelectorAll('input[data-date]').forEach(function (el) {
            window.flatpickr(el, {
                locale: locale || 'default',
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'j F Y',
                disableMobile: true
            });
        });
    }

    /* ---------------- Verwijderbevestiging (modal) ---------------- */
    function initConfirmModal() {
        var modal = document.getElementById('confirm-modal');
        if (!modal) return;
        var message = modal.querySelector('#confirm-message');
        var accept = modal.querySelector('[data-confirm-accept]');
        var cancel = modal.querySelector('[data-confirm-cancel]');
        var pendingForm = null;

        function open(form) {
            pendingForm = form;
            message.textContent = form.getAttribute('data-confirm') || '';
            modal.hidden = false;
            requestAnimationFrame(function () { modal.classList.add('is-open'); });
            accept.focus();
        }

        function close() {
            modal.classList.remove('is-open');
            pendingForm = null;
            window.setTimeout(function () { modal.hidden = true; }, 180);
        }

        document.addEventListener('submit', function (e) {
            var form = e.target;
            if (!(form instanceof HTMLFormElement)) return;
            if (!form.hasAttribute('data-confirm')) return;
            e.preventDefault();
            open(form);
        }, true);

        accept.addEventListener('click', function () {
            if (pendingForm) {
                var form = pendingForm;
                pendingForm = null;
                modal.classList.remove('is-open');
                modal.hidden = true;
                form.submit();
            }
        });
        cancel.addEventListener('click', close);
        modal.addEventListener('click', function (e) {
            if (e.target === modal) close();
        });
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape' && !modal.hidden) close();
        });
    }

    /* ---------------- Toasts ---------------- */
    function dismissToast(toast) {
        toast.classList.add('toast-leaving');
        window.setTimeout(function () { toast.remove(); }, 260);
    }

    TW.toast = function (text) {
        var region = document.getElementById('toast-region');
        if (!region) return;
        var toast = document.createElement('div');
        toast.className = 'toast';
        toast.innerHTML = '<i data-lucide="check-circle-2" class="h-4 w-4 shrink-0" style="color:var(--ok); margin-top:0.1rem;"></i><span></span>';
        toast.querySelector('span').textContent = text;
        region.appendChild(toast);
        TW.icons();
        window.setTimeout(function () { dismissToast(toast); }, 4500);
    };

    function initToasts() {
        document.querySelectorAll('[data-toast]').forEach(function (toast) {
            toast.addEventListener('click', function () { dismissToast(toast); });
            window.setTimeout(function () { dismissToast(toast); }, 4500);
        });
    }

    /* ---------------- Zoeken/sorteren in tabellen (List.js) ---------------- */
    function initTables() {
        if (!window.List) return;
        document.querySelectorAll('[data-list]').forEach(function (root) {
            var valueNames;
            try { valueNames = JSON.parse(root.getAttribute('data-list')); } catch (e) { return; }
            var list = new window.List(root, { valueNames: valueNames });

            var countEl = root.querySelector('[data-list-count]');
            var emptyEl = root.querySelector('[data-list-empty]');
            function refresh() {
                if (countEl) countEl.textContent = String(list.matchingItems.length);
                if (emptyEl) emptyEl.classList.toggle('hidden', list.matchingItems.length > 0);
            }
            list.on('updated', refresh);
            refresh();

            var pills = root.querySelectorAll('[data-filter-status]');
            pills.forEach(function (pill) {
                pill.addEventListener('click', function () {
                    pills.forEach(function (p) { p.classList.remove('is-active'); });
                    pill.classList.add('is-active');
                    var wanted = pill.getAttribute('data-filter-status');
                    if (!wanted) {
                        list.filter();
                    } else {
                        list.filter(function (item) {
                            return item.values()['q-status'] === wanted;
                        });
                    }
                });
            });
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        TW.icons();
        initTheme();
        initLightbox();
        initSelects();
        initDatePickers();
        initConfirmModal();
        initToasts();
        initTables();
    });
})();
