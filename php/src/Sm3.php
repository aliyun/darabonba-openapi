<?php

// This file is auto-generated, don't edit it. Thanks.

namespace Darabonba\OpenApi;

/**
 * SM3 cryptographic hash algorithm implementation
 * SM3 is a cryptographic hash function standard of China
 */
class Sm3
{
    private static $IV = [
        0x7380166f, 0x4914b2b9, 0x172442d7, 0xda8a0600,
        0xa96f30bc, 0x163138aa, 0xe38dee4d, 0xb0fb0e4e
    ];

    private static $T_0_15 = 0x79cc4519;
    private static $T_16_63 = 0x7a879d8a;

    /**
     * Calculate SM3 hash of the message
     *
     * @param string $message The message to hash
     * @return string The hexadecimal hash string
     */
    public function sign($message)
    {
        $len = strlen($message);
        $bitLen = $len * 8;
        
        // Padding
        $message .= "\x80";
        $padLen = (56 - (($len + 1) % 64)) % 64;
        $message .= str_repeat("\x00", $padLen);
        
        // Append length
        $message .= pack('N', $bitLen >> 32);
        $message .= pack('N', $bitLen & 0xFFFFFFFF);
        
        // Process message in 512-bit blocks
        $v = self::$IV;
        $blocks = str_split($message, 64);
        
        foreach ($blocks as $block) {
            $v = $this->cf($v, $block);
        }
        
        // Convert to hex string
        $result = '';
        foreach ($v as $word) {
            $result .= sprintf('%08x', $word);
        }
        
        return $result;
    }

    /**
     * Compression function
     */
    private function cf($v, $block)
    {
        $w = [];
        $w1 = [];
        
        // Expand message block
        for ($i = 0; $i < 16; $i++) {
            $w[$i] = unpack('N', substr($block, $i * 4, 4))[1];
        }
        
        for ($i = 16; $i < 68; $i++) {
            $w[$i] = $this->p1(
                $w[$i - 16] ^ $w[$i - 9] ^ $this->rotl($w[$i - 3], 15)
            ) ^ $this->rotl($w[$i - 13], 7) ^ $w[$i - 6];
        }
        
        for ($i = 0; $i < 64; $i++) {
            $w1[$i] = $w[$i] ^ $w[$i + 4];
        }
        
        // Compression
        $a = $v[0];
        $b = $v[1];
        $c = $v[2];
        $d = $v[3];
        $e = $v[4];
        $f = $v[5];
        $g = $v[6];
        $h = $v[7];
        
        for ($i = 0; $i < 64; $i++) {
            $ss1 = $this->rotl(
                $this->add32(
                    $this->add32($this->rotl($a, 12), $e),
                    $this->rotl($this->t($i), $i)
                ),
                7
            );
            
            $ss2 = $ss1 ^ $this->rotl($a, 12);
            $tt1 = $this->add32(
                $this->add32(
                    $this->add32($this->ff($a, $b, $c, $i), $d),
                    $ss2
                ),
                $w1[$i]
            );
            
            $tt2 = $this->add32(
                $this->add32(
                    $this->add32($this->gg($e, $f, $g, $i), $h),
                    $ss1
                ),
                $w[$i]
            );
            
            $d = $c;
            $c = $this->rotl($b, 9);
            $b = $a;
            $a = $tt1;
            $h = $g;
            $g = $this->rotl($f, 19);
            $f = $e;
            $e = $this->p0($tt2);
        }
        
        return [
            $a ^ $v[0],
            $b ^ $v[1],
            $c ^ $v[2],
            $d ^ $v[3],
            $e ^ $v[4],
            $f ^ $v[5],
            $g ^ $v[6],
            $h ^ $v[7]
        ];
    }

    private function ff($x, $y, $z, $j)
    {
        if ($j < 16) {
            return $x ^ $y ^ $z;
        }
        return ($x & $y) | ($x & $z) | ($y & $z);
    }

    private function gg($x, $y, $z, $j)
    {
        if ($j < 16) {
            return $x ^ $y ^ $z;
        }
        return ($x & $y) | ((~$x) & $z);
    }

    private function t($j)
    {
        if ($j < 16) {
            return self::$T_0_15;
        }
        return self::$T_16_63;
    }

    private function rotl($x, $n)
    {
        $n = $n % 32;
        return (($x << $n) | ($x >> (32 - $n))) & 0xFFFFFFFF;
    }

    private function p0($x)
    {
        return $x ^ $this->rotl($x, 9) ^ $this->rotl($x, 17);
    }

    private function p1($x)
    {
        return $x ^ $this->rotl($x, 15) ^ $this->rotl($x, 23);
    }

    private function add32($a, $b)
    {
        return ($a + $b) & 0xFFFFFFFF;
    }
}