const { Scrambow } = require('scrambow');

window.scrambleGenerator =
{
    getDefaultScramble: () =>
    {
        const scrambler = new Scrambow();
        scrambler.setType('333');
        return scrambler.get()[0].scramble_string;
    }
};
