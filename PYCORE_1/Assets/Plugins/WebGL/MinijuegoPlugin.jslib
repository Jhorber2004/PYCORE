mergeInto(LibraryManager.library, {
  AbrirMinijuegoHTML: function(url) {
    var urlStr = UTF8ToString(url);
    var iframe = document.createElement('iframe');
    iframe.id = 'minijuego-frame';
    iframe.src = urlStr;
    iframe.style.position = 'fixed';
    iframe.style.top = '0';
    iframe.style.left = '0';
    iframe.style.width = '100%';
    iframe.style.height = '100%';
    iframe.style.border = 'none';
    iframe.style.zIndex = '9999';
    document.body.appendChild(iframe);
  },
  CerrarMinijuegoHTML: function() {
    var iframe = document.getElementById('minijuego-frame');
    if (iframe) iframe.remove();
  }
});