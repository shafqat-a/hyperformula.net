const ROWS = 50;
const COLS = 26; // A-Z
const COL_WIDTH = 100;
const ROW_HEADER_WIDTH = 40;

const spreadsheetHeader = document.getElementById('spreadsheet-header');
const spreadsheetBody = document.getElementById('spreadsheet-body');
const formulaInput = document.getElementById('formula-input');
const selectedCellIdIndicator = document.getElementById('selected-cell-id');

let selectedCell = null;
let cellData = {}; // Address -> Current Value

// Initialize Grid Layout
function initGrid() {
    // CSS Grid Template
    const gridTemplateCols = `${ROW_HEADER_WIDTH}px repeat(${COLS}, ${COL_WIDTH}px)`;
    spreadsheetHeader.style.gridTemplateColumns = gridTemplateCols;
    spreadsheetBody.style.gridTemplateColumns = gridTemplateCols;

    // 1. Headers
    // Corner
    const corner = document.createElement('div');
    corner.className = 'corner-header';
    spreadsheetHeader.appendChild(corner);

    // Column Headers (A-Z)
    for (let c = 0; c < COLS; c++) {
        const colHeader = document.createElement('div');
        colHeader.className = 'header-cell';
        colHeader.textContent = String.fromCharCode(65 + c);
        spreadsheetHeader.appendChild(colHeader);
    }

    // 2. Body (Rows)
    for (let r = 1; r <= ROWS; r++) {
        // Row Header
        const rowHeader = document.createElement('div');
        rowHeader.className = 'row-header';
        rowHeader.textContent = r;
        spreadsheetBody.appendChild(rowHeader);

        // Cells
        for (let c = 0; c < COLS; c++) {
            const colLetter = String.fromCharCode(65 + c);
            const address = `${colLetter}${r}`;
            
            const cell = document.createElement('div');
            cell.className = 'cell';
            cell.dataset.address = address;
            cell.tabIndex = 0; // Focusable
            cell.id = `cell-${address}`;
            
            cell.addEventListener('click', () => selectCell(address));
            cell.addEventListener('dblclick', () => enterEditMode(address));
            
            // On Enter key
            cell.addEventListener('keydown', (e) => {
                if(e.key === 'Enter') {
                    e.preventDefault();
                    if (cell.isContentEditable) {
                        commitEdit(address);
                    } else {
                        enterEditMode(address);
                    }
                }
            });
            
            // Blur handling (if editing)
            cell.addEventListener('blur', () => {
                if (cell.isContentEditable) {
                    commitEdit(address);
                }
            });

            spreadsheetBody.appendChild(cell);
        }
    }
}

function selectCell(address) {
    if (selectedCell) {
        document.getElementById(`cell-${selectedCell}`).classList.remove('selected');
    }
    selectedCell = address;
    const cellEl = document.getElementById(`cell-${address}`);
    cellEl.classList.add('selected');
    
    selectedCellIdIndicator.textContent = address;
    formulaInput.disabled = false;
    
    // If not editing, assume formula input should clear or show formula? 
    // Usually shows formula if exists. We assume input is just formula for now.
    // We don't have stored formula in frontend state yet, only value.
    // Ideally backend returns { value, formula }.
    // For now, empty formula bar or put value.
    formulaInput.value = cellEl.textContent; 
    // If we want formula, we need to fetch it or store it.
    // Let's assume for v1 only Values are synced primarily. 
    // Wait, editing overwrites with formula. 
    // Todo: Enhance API to return Formula.
}

function enterEditMode(address) {
    const cellEl = document.getElementById(`cell-${address}`);
    cellEl.contentEditable = true;
    cellEl.focus();
    // Ideally show source formula.
}

async function commitEdit(address) {
    const cellEl = document.getElementById(`cell-${address}`);
    cellEl.contentEditable = false;
    const content = cellEl.textContent;
    
    // Send to API
    await updateCell(address, content);
}

// API Calls
async function updateCell(address, formula) {
    try {
        const response = await fetch('/api/cells', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ address, formula })
        });
        
        if (!response.ok) {
            console.error("Error updating cell");
            return;
        }

        const data = await response.json();
        // Update this cell
        updateCellDisplay(data.address, data.value);
        
        // Refresh all cells to catch dependencies
        // Optimization: backend should return list of changed cells.
        // For now, fetch all.
        await fetchCells(); 

    } catch (e) {
        console.error(e);
    }
}

async function fetchCells() {
    try {
        const response = await fetch('/api/cells');
        const data = await response.json();
        // data is { "A1": "Value", ... }
        for (const [addr, val] of Object.entries(data)) {
            updateCellDisplay(addr, val);
        }
    } catch (e) {
        console.error(e);
    }
}

function updateCellDisplay(address, value) {
    const cell = document.getElementById(`cell-${address}`);
    if (cell) {
        cell.textContent = value;
    }
}

// Formula Bar Input
formulaInput.addEventListener('keydown', (e) => {
    if (e.key === 'Enter' && selectedCell) {
        updateCell(selectedCell, formulaInput.value);
    }
});

// Start
initGrid();
fetchCells();
