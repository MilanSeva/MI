// English <-> Gujarati phonetic transliteration for product names. No external library.
//
// English -> Gujarati (rule based, so vowel length must be typed where it can't be guessed):
//   aa/ee/oo = long vowels (kaaju -> કાજુ), consonant clusters join with ્ (pista -> પિસ્તા),
//   a word-final "a" becomes ા (ha -> હા) and a word-final "i" becomes ી (haldi -> હલ્દી),
//   n/m before a matching consonant becomes ં (chandan -> ચંદન).
// Gujarati -> English: letter-by-letter with silent-"a" (schwa) deletion and capitalised words
//   (બદામ -> Badam, હળદર -> Haldar, મગફળી -> Magfali).
var GujaratiTransliterator = (function () {
    'use strict';

    // ---------- English -> Gujarati ----------

    // [roman, gujarati]; longest roman spellings are tried first
    var CONSONANTS = [
        ['ksh', 'ક્ષ'], ['chh', 'છ'], ['ck', 'ક'],
        ['kh', 'ખ'], ['gh', 'ઘ'], ['ch', 'ચ'], ['jh', 'ઝ'], ['th', 'થ'], ['dh', 'ધ'],
        ['ph', 'ફ'], ['bh', 'ભ'], ['sh', 'શ'],
        ['k', 'ક'], ['g', 'ગ'], ['c', 'ક'], ['j', 'જ'], ['z', 'ઝ'], ['t', 'ત'], ['d', 'દ'],
        ['n', 'ન'], ['p', 'પ'], ['f', 'ફ'], ['b', 'બ'], ['m', 'મ'], ['y', 'ય'], ['r', 'ર'],
        ['l', 'લ'], ['v', 'વ'], ['w', 'વ'], ['s', 'સ'], ['h', 'હ'], ['q', 'ક'], ['x', 'ક્સ']
    ];
    // [roman, independent letter, vowel sign after a consonant]
    var VOWELS = [
        ['aa', 'આ', 'ા'], ['ai', 'ઐ', 'ૈ'], ['au', 'ઔ', 'ૌ'], ['ee', 'ઈ', 'ી'], ['ii', 'ઈ', 'ી'],
        ['oo', 'ઊ', 'ૂ'], ['uu', 'ઊ', 'ૂ'],
        ['a', 'અ', ''], ['i', 'ઇ', 'િ'], ['u', 'ઉ', 'ુ'], ['e', 'એ', 'ે'], ['o', 'ઓ', 'ો']
    ];
    var HALANT = '્';
    var ANUSVARA = 'ં';
    // n before these becomes ં (ganthiya, chandan); m before these becomes ં (kampani)
    var NASAL_BEFORE_N = ['k', 'kh', 'g', 'gh', 'ch', 'chh', 'j', 'jh', 'z', 't', 'th', 'd', 'dh'];
    var NASAL_BEFORE_M = ['p', 'ph', 'f', 'b', 'bh'];

    var ROMAN_TOKENS = CONSONANTS.map(function (c) { return { roman: c[0], type: 'C', guj: c[1] }; })
        .concat(VOWELS.map(function (v) { return { roman: v[0], type: 'V', letter: v[1], sign: v[2] }; }))
        .sort(function (a, b) { return b.roman.length - a.roman.length; });

    function tokenizeRoman(word) {
        var tokens = [];
        var pos = 0;
        while (pos < word.length) {
            var match = null;
            for (var i = 0; i < ROMAN_TOKENS.length; i++) {
                if (word.startsWith(ROMAN_TOKENS[i].roman, pos)) { match = ROMAN_TOKENS[i]; break; }
            }
            if (!match) { pos++; continue; } // unreachable for a-z, kept for safety
            tokens.push(match);
            pos += match.roman.length;
        }
        return tokens;
    }

    function romanWordToGujarati(word) {
        var tokens = tokenizeRoman(word.toLowerCase());
        var out = '';
        for (var i = 0; i < tokens.length; i++) {
            var t = tokens[i];
            var next = tokens[i + 1];
            var isLast = i === tokens.length - 1;
            if (t.type === 'C') {
                // n/m followed by a matching consonant, after a vowel sound -> anusvara
                if (i > 0 && next && next.type === 'C' &&
                    ((t.roman === 'n' && NASAL_BEFORE_N.indexOf(next.roman) >= 0) ||
                     (t.roman === 'm' && NASAL_BEFORE_M.indexOf(next.roman) >= 0))) {
                    out += ANUSVARA;
                    continue;
                }
                out += t.guj;
                if (next && next.type === 'C') out += HALANT; // cluster: pista -> પિસ્તા
                continue;
            }
            // vowel
            var afterConsonant = i > 0 && tokens[i - 1].type === 'C';
            if (!afterConsonant) {
                out += t.letter;
            } else if (isLast && t.roman === 'a') {
                out += 'ા'; // ha -> હા, pista -> પિસ્તા
            } else if (isLast && t.roman === 'i') {
                out += 'ી'; // haldi -> હલ્દી, ghi -> ઘી
            } else {
                out += t.sign; // '' for a short "a" (inherent vowel)
            }
        }
        return out;
    }

    function toGujarati(text) {
        return (text || '').replace(/[A-Za-z]+/g, romanWordToGujarati);
    }

    // ---------- Gujarati -> English ----------

    var GUJ_CONSONANTS = {
        'ક': 'k', 'ખ': 'kh', 'ગ': 'g', 'ઘ': 'gh', 'ઙ': 'ng', 'ચ': 'ch', 'છ': 'chh', 'જ': 'j', 'ઝ': 'z',
        'ઞ': 'ny', 'ટ': 't', 'ઠ': 'th', 'ડ': 'd', 'ઢ': 'dh', 'ણ': 'n', 'ત': 't', 'થ': 'th', 'દ': 'd',
        'ધ': 'dh', 'ન': 'n', 'પ': 'p', 'ફ': 'f', 'બ': 'b', 'ભ': 'bh', 'મ': 'm', 'ય': 'y', 'ર': 'r',
        'લ': 'l', 'વ': 'v', 'શ': 'sh', 'ષ': 'sh', 'સ': 's', 'હ': 'h', 'ળ': 'l'
    };
    // Long and short vowels both map to the plain letter: કાજુ -> Kaju reads better than Kaaju
    var GUJ_VOWEL_SIGNS = {
        'ા': 'a', 'િ': 'i', 'ી': 'i', 'ુ': 'u', 'ૂ': 'u', 'ૃ': 'ru', 'ે': 'e', 'ૈ': 'ai',
        'ો': 'o', 'ૌ': 'au', 'ૅ': 'e', 'ૉ': 'o'
    };
    var GUJ_VOWELS = {
        'અ': 'a', 'આ': 'a', 'ઇ': 'i', 'ઈ': 'i', 'ઉ': 'u', 'ઊ': 'u', 'ઋ': 'ru', 'એ': 'e', 'ઐ': 'ai',
        'ઓ': 'o', 'ઔ': 'au', 'ઍ': 'e', 'ઑ': 'o'
    };
    var GUJ_DIGITS = '૦૧૨૩૪૫૬૭૮૯';

    // One syllable per entry: { c: consonant(s), v: vowel ('' = none), schwa: v is the inherent "a", nasal, post }
    function gujaratiWordToRoman(word) {
        var segs = [];
        for (var i = 0; i < word.length; i++) {
            var ch = word[i];
            var last = segs[segs.length - 1];
            if (GUJ_CONSONANTS[ch] !== undefined) {
                // જ્ઞ is conventionally written "gn" (gnan), not "jny"
                if (ch === 'ઞ' && last && last.c === 'j' && last.v === '') {
                    last.c = 'g';
                    segs.push({ c: 'n', v: 'a', schwa: true, nasal: '', post: '' });
                } else if (ch === 'છ' && last && last.c === 'ch' && last.v === '') {
                    // ચ્છ is written "chh" (machhar), not "chchh"
                    last.c = '';
                    segs.push({ c: 'chh', v: 'a', schwa: true, nasal: '', post: '' });
                } else {
                    segs.push({ c: GUJ_CONSONANTS[ch], v: 'a', schwa: true, nasal: '', post: '' });
                }
            } else if (GUJ_VOWEL_SIGNS[ch] !== undefined && last) {
                last.v = GUJ_VOWEL_SIGNS[ch];
                last.schwa = false;
            } else if (ch === HALANT && last) {
                last.v = '';
                last.schwa = false;
            } else if (GUJ_VOWELS[ch] !== undefined) {
                segs.push({ c: '', v: GUJ_VOWELS[ch], schwa: false, nasal: '', post: '' });
            } else if ((ch === 'ં' || ch === 'ઁ') && last) {
                last.nasal = 'n';
            } else if (ch === 'ઃ' && last) {
                last.post = 'h';
            }
            // nukta and anything else are dropped
        }

        var n = segs.length;
        // Silent final "a": બદામ -> badam, વિશિષ્ટ -> vishisht. It is sounded in a one-letter word, after ય
        // (ભારતીય -> bhartiya) and after a cluster ending in ર (પત્ર -> patra, વસ્ત્ર -> vastra).
        if (n > 1 && segs[n - 1].schwa) {
            var finalSeg = segs[n - 1];
            var afterCluster = segs[n - 2].v === '';
            var keepA = finalSeg.c === 'y' || (afterCluster && finalSeg.c === 'r');
            if (!keepA) finalSeg.v = '';
        }
        // A word-final ં only nasalises the vowel: ધોવાનું -> dhovanu
        if (n > 0 && segs[n - 1].v !== '') segs[n - 1].nasal = '';
        // Silent medial "a" between vowel+consonant and consonant+vowel: હળદર -> haldar, મગફળી -> magfali
        for (var j = 1; j < n - 1; j++) {
            var s = segs[j];
            // a preceding ં already closes that syllable, so this "a" is sounded: કંપની -> kampani
            if (s.schwa && s.v === 'a' && s.c && !s.nasal &&
                segs[j - 1].v !== '' && !segs[j - 1].nasal && segs[j + 1].c && segs[j + 1].v !== '') {
                s.v = '';
            }
        }

        var out = '';
        for (var k = 0; k < n; k++) {
            var seg = segs[k];
            var nasal = seg.nasal;
            // ં before p/b/m sounds is read as "m": કંપની -> kampani
            if (nasal && segs[k + 1] && /^[pbm]/.test(segs[k + 1].c)) nasal = 'm';
            out += seg.c + seg.v + nasal + seg.post;
        }
        return out.charAt(0).toUpperCase() + out.slice(1);
    }

    function toEnglish(text) {
        return (text || '')
            .replace(/[ઁ-ૣ]+/g, gujaratiWordToRoman)
            .replace(/[૦-૯]/g, function (d) { return String(GUJ_DIGITS.indexOf(d)); });
    }

    return { toGujarati: toGujarati, toEnglish: toEnglish };
})();

// Wires a modal's Product Name and Gujarati Name inputs so typing in one fills the other.
// A field is only overwritten while it is empty or still holds an auto-filled value; once the user
// types in it, it is theirs. Flags reset each time the modal opens.
function initNameTransliteration($modal, englishSelector, gujaratiSelector) {
    var $english = $modal.find(englishSelector);
    var $gujarati = $modal.find(gujaratiSelector);

    function link($source, $target, convert) {
        $source.on('input', function () {
            $source.data('autofilled', false); // user typed here, so never overwrite this field
            if ($target.val() === '' || $target.data('autofilled')) {
                $target.val(convert($source.val())).data('autofilled', true);
            }
        });
    }
    link($english, $gujarati, GujaratiTransliterator.toGujarati);
    link($gujarati, $english, GujaratiTransliterator.toEnglish);

    $modal.on('show.bs.modal', function () {
        $english.data('autofilled', false);
        $gujarati.data('autofilled', false);
    });
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = GujaratiTransliterator; // for Node-based testing only
}
