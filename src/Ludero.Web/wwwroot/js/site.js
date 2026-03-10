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
document.querySelectorAll('.modal-overlay').forEach(function(overlay) {
  overlay.addEventListener('click', function(e) {
    if (e.target === this) this.classList.remove('open');
  });
});

// ── Query string checks on page load ──
document.addEventListener('DOMContentLoaded', function() {
  var params = new URLSearchParams(window.location.search);

  // Show factsheet success state if ?factsheet=true
  if (params.get('factsheet') === 'true') {
    var form = document.getElementById('factsheetForm');
    var success = document.getElementById('factsheet-success');
    if (form) form.style.display = 'none';
    if (success) success.classList.add('show');
    openModal('modal-factsheet');
  }
});
