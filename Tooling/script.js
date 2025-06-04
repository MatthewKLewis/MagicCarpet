const canvas = document.querySelector("#canvas");
const ctx = canvas.getContext("2d");
const paletteInfoElement = document.querySelector("#palette-info");

class CastleGrid {
  constructor(sL) {
    this.sideLength = sL;
    this.tiles = new Array(sL);

    for (let y = 0; y < sL; y++) {
      this.tiles[y] = new Array(sL).fill(0);
    }

    this.fillGridWithZeros();
    //console.log(this.tiles);
  }

  fillGridWithZeros() {
    for (let y = 0; y < this.sideLength; y++) {
      for (let x = 0; x < this.sideLength; x++) {
        this.tiles[x][y] = 0;
      }
    }
  }

  getValueAt(x, y) {
    return this.tiles[x][y];
  }

  changeValueAt(x, y, value) {
    this.tiles[x][y] = value;
  }

  printTiles() {
    console.log("VERTEX HEIGHT");
    let retStr = "{\n";
    for (let y = 0; y < this.sideLength; y++) {
      retStr += "{";
      for (let x = 0; x < this.sideLength; x++) {
        retStr += this.tiles[x][y] + ",";
      }
      retStr += "},\n";
    }
    retStr += "};";
    console.log(retStr);

    console.log("VERTEX COLOR");
    let colorStr = "{\n";
    for (let y = 0; y < this.sideLength; y++) {
      colorStr += "{";
      for (let x = 0; x < this.sideLength; x++) {
        colorStr += "Color.gray,";
      }
      colorStr += "},\n";
    }
    colorStr += "};";
    console.log(colorStr);

    console.log("SQUARE UV");
    let uvBasisRemapStr = "{\n";
    for (let y = 0; y < this.sideLength - 1; y++) {
      uvBasisRemapStr += "{";
      for (let x = 0; x < this.sideLength - 1; x++) {
        uvBasisRemapStr += "BLANK,";
      }
      uvBasisRemapStr += "},\n";
    }
    uvBasisRemapStr += "};";
    console.log(uvBasisRemapStr);

    console.log("SQUARE TRIFLIP");
    let triFlipStr = "{\n";
    for (let y = 0; y < this.sideLength - 1; y++) {
      triFlipStr += "{";
      for (let x = 0; x < this.sideLength - 1; x++) {
        triFlipStr += "false,";
      }
      triFlipStr += "},\n";
    }
    triFlipStr += "};";
    console.log(triFlipStr);
  }
}

//#region
function render() {
  ctx.clearRect(0, 0, castleGrid.sideLength, castleGrid.sideLength);
  for (let y = 0; y < castleGrid.sideLength; y++) {
    for (let x = 0; x < castleGrid.sideLength; x++) {
      drawSquare(x, y);
    }
  }
}

function drawSquare(x, y) {
  ctx.fillStyle =
    "#" +
    castleGrid.tiles[x][y] +
    castleGrid.tiles[x][y] +
    castleGrid.tiles[x][y];
  ctx.fillRect(x * TILE_PIXELS, y * TILE_PIXELS, TILE_PIXELS, TILE_PIXELS);
}

function drawIndicator(tileX, tileY) {
  for (let YY = 0; YY < TILE_PIXELS; YY++) {
    for (let XX = 0; XX < TILE_PIXELS; XX++) {
      if (
        XX == 0 ||
        YY == 0 ||
        XX == TILE_PIXELS - 1 ||
        YY == TILE_PIXELS - 1
      ) {
        ctx.fillStyle = "Orange";
        ctx.fillRect(tileX * TILE_PIXELS + XX, tileY * TILE_PIXELS + YY, 1, 1);
      }
    }
  }
}

function setHeight(value) {
  CURRENT_HEIGHT = value;
  paletteInfoElement.innerText =
    "Painting Current Height: " + CURRENT_HEIGHT + "m";
}

function resetCanvas(length) {
  castleGrid = new CastleGrid(length);

  canvas.style.width = castleGrid.sideLength * TILE_PIXELS;
  canvas.style.height = castleGrid.sideLength * TILE_PIXELS;
  canvas.width = castleGrid.sideLength * TILE_PIXELS;
  canvas.height = castleGrid.sideLength * TILE_PIXELS;
  canvLeft = canvas.offsetLeft + canvas.clientLeft;
  canvTop = canvas.offsetTop + canvas.clientTop;

  render();
}

function printTiles(event) {
  castleGrid.printTiles();
}

canvas.addEventListener(
  "click",
  function (event) {
    var x = event.pageX - canvLeft,
      y = event.pageY - canvTop;
    var gridX = Math.floor(x / TILE_PIXELS),
      gridY = Math.floor(y / TILE_PIXELS);

    //SET THE HEIGHT IN THE ARRAY
    castleGrid.changeValueAt(gridX, gridY, CURRENT_HEIGHT);
    render();
  },
  false
);

canvas.addEventListener(
  "mousemove",
  function (event) {
    var x = event.pageX - canvLeft,
      y = event.pageY - canvTop;
    var gridX = Math.floor(x / TILE_PIXELS),
      gridY = Math.floor(y / TILE_PIXELS);
    render();
    drawIndicator(gridX, gridY);
  },
  false
);

//#endregion

let TILE_PIXELS = 32;
let CURRENT_HEIGHT = 1;
let castleGrid = new CastleGrid(4);

paletteInfoElement.innerText = "Painting at Height: " + CURRENT_HEIGHT + "m";

resetCanvas();
render();
