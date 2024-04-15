window.createDynamicCanvas = function () {
    var container = document.querySelector('#canvas-container');
    if (!container) return; // Exit if container not found

    // Clear previous canvas if it exists
    while (container.firstChild) {
        container.removeChild(container.firstChild);
    }

    var canvas = document.createElement('canvas');
    var ctx = canvas.getContext('2d');
    container.appendChild(canvas); // Append canvas to container

    // Function to apply canvas styles
    function applyCanvasStyles() {
        ctx.strokeStyle = '#67dcff';
        ctx.lineWidth = 3; // Adjust for thicker lines
    }

    // Make canvas responsive to container and viewport
    function resizeCanvas() {
        canvas.width = container.offsetWidth;
        canvas.height = container.offsetHeight;
        applyCanvasStyles(); // Re-apply styles after resizing
    }

    resizeCanvas();
    window.addEventListener('resize', resizeCanvas);

    var K = 0.008;
    var B = 80;
    var move = 0;

    applyCanvasStyles(); // Apply styles initially

    function frame() {
        requestAnimationFrame(frame);
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        for (var x = 0; x <= canvas.width + 80; x++) {
            if (x % 20 == Math.floor(move)) {
                ctx.beginPath();
                ctx.moveTo(x, Math.cos(x * K) * B + canvas.height / 2);
                ctx.lineTo(x - 80, -Math.cos((x - 80) * K) * B + canvas.height / 2);
                ctx.stroke();
            }
        }

        move += 0.5; // This will create the moving effect
        if (move >= 20) move = 0;
    }

    frame();
};
