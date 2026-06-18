// Most Common Barcodes: UPC-A and EAN-13

// Barcode Reader
// This creates an engine that can read barcodes from a camera
// ZXing: the library that was loaded from the CDN (Content Delivery network)
// BrowserMultiFormatReader: A class that can scan many barcode types (QR, UPC, EANS Code128 etc.)
const codeReader = new ZXing.BrowserMultiFormatReader();

// This line prevents mulitple camera streams and button clicks
// sacnning: flag to track whether scanning is active
let scanning = false;

// Once the AddGame page is loaded,
// document: the whole ____ HTML page
// addEventListener: listens for an event
// DOMConetentLoaded: fires when the HTML is fully loaded
// () => {} arrow function (call back)
document.addEventListener("DOMContentLoaded", () => {

    // Grab HTML Elements
    const videoElement = document.getElementById('video');  // Gets the video id 'video' (Where the camera stream will display)
    const resultElement = document.getElementById('result'); // Gets the span ID 'result' Display Ba

    // Get the input field (This is what get's autofilled)
    const barcodeInput =
        document.getElementById('CheckInBarcode') ||
        document.getElementById('BarcodeInput') ||
        document.getElementById('Barcode') ||
        document.getElementById('Game_Barcode'); 
    const gameBarcodeInput = document.getElementById('Game_Barcode');
    const startButton = document.getElementById('startScan'); //This Gets the 'Scan Barcode' Button

    // This makes sure the code stops if the button doesn't exist
    // This prevent Script from running on other pages
    if (!startButton) return;

    // This waits for the user to click the scan button before unning addEventListener
    startButton.addEventListener('click', () => {

        // If it's already scanning, it does nothing (Multi Camera Stream Prevention)
        if (scanning) return;

        // Marks scanning as active 
        scanning = true;

        // This sets a timeout to stop scanning after 10 seconds
        const timeout = setTimeout(() => {
            // If scanning is still active after 10 seconds, it stops the scan and resets the reader
            if (scanning) {
                codeReader.reset(); // Stops the camera and scanning loop
                scanning = false; // Unlocks the scan button

                resultElement.textContent = "Scan timed out"; // Displays a message to the user that the scan timed out
            }
        }, 10000); // 10 seconds

        // This starts the Camera and Scanning
        codeReader.decodeFromVideoDevice(
            null, // Allows ZXing to autopick the camera
            videoElement, // The window element where the camera shows
            (result, err) => { // This function loops (result: barcode found, err: nothing found or scan error)
                if (result) {
                    const barcode = result.text; //stores the actual barcode string in a variable

                    // Updates the feilds in the form (Code: 1234567890)
                    resultElement.textContent = barcode;

                    // This fills the form automatically
                    barcodeInput.value = barcode;

                    // Also populate Game.Barcode
                    if (gameBarcodeInput) {
                        gameBarcodeInput.value = barcode;
                    }

                    // Automatically submit barcode lookup form
                    barcodeInput.form.submit();

                    // Turns off the Camera and stops the scanning loop
                    codeReader.reset();
                    scanning = false; // Unlocks the scan button
                }

                // Catches Exceptions (This Loops)
                if (err && !(err instanceof ZXing.NotFoundException)) {
                    console.error(err);
                }
            }
        );
    });
});