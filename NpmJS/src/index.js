const { Scrambow } = require('scrambow');

window.scrambleGenerator =
{
    getDefaultScramble: (eventType) =>
    {
        const scrambler = new Scrambow();
        scrambler.setType(eventType);
        return scrambler.get()[0].scramble_string;
    }
};
