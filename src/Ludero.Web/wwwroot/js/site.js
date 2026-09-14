// ── Mobile nav ──
function toggleMobileNav() {
  document.getElementById('navLinks').classList.toggle('open');
}

// ── Modals ──
function openModal(id) {
  document.getElementById(id).classList.add('open');
}

function closeModal(id) {
  document.getElementById(id).classList.remove('open');
}

// Close modal on overlay click
document.querySelectorAll('.modal-overlay').forEach(function (overlay) {
  overlay.addEventListener('click', function (e) {
    if (e.target === this) this.classList.remove('open');
  });
});

// ── Query string checks on page load ──
document.addEventListener('DOMContentLoaded', function () {
  var params = new URLSearchParams(window.location.search);
  var hash = window.location.hash;

  // Show factsheet success state if ?factsheet=true
  if (params.get('factsheet') === 'true') {
    var form = document.getElementById('factsheetForm');
    var success = document.getElementById('factsheet-success');
    if (form) form.style.display = 'none';
    if (success) success.classList.add('show');
    openModal('modal-factsheet');
  } else if (params.has('factsheet')) {
    openModal('modal-factsheet');
  }

  if (params.has('privacy')) {
    openModal('modal-privacy');
  }
  if (params.has('cookies')) {
    openModal('modal-cookies');
  }

  // Generic modal opening via ?modal=name or #modal-name
  var modalParam = params.get('modal');
  if (modalParam) {
    var modalId = 'modal-' + modalParam;
    if (document.getElementById(modalId)) {
      openModal(modalId);
    }
  } else if (hash && hash.startsWith('#modal-')) {
    var modalId = hash.substring(1);
    if (document.getElementById(modalId)) {
      openModal(modalId);
    }
  }
});
