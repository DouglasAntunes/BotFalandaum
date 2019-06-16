"use strict";

class Sound {
    constructor(name, weight) {
        this.name = name;
        this.weight = weight;
    }

    set buffer(buffer) {
        this.buffer = buffer;
    }

    get buffer() {
        return this.buffer;
    }

    get name() {
        return this.name;
    }

    get weight() {
        return this.weight;
    }
};