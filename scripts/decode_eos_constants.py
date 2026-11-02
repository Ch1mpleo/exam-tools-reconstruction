"""Static Confuser constant/resource decoding for the exact supplied EOS build.
No assembly loading or application execution. Parameters are observed in IL.
"""
from pathlib import Path
import hashlib, json, lzma, re, struct

ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / 'src/EOS.Analysis'
text = (SOURCE / '-Module-.cs').read_text(encoding='utf-8-sig')
mask = 0xffffffff

def unpack_array(length, seed, right_shift):
    m = re.search(r'new uint\[' + str(length) + r'\]\s*\{(.*?)\};', text, re.S)
    words = [int(x) for x in re.findall(r'(\d+)u', m[1])]
    assert len(words) == length
    key = []
    for _ in range(16):
        seed ^= seed >> right_shift
        seed = (seed ^ (seed << 25)) & mask
        seed ^= seed >> 27
        key.append(seed)
    out = bytearray()
    for start in range(0, length, 16):
        for i, word in enumerate(words[start:start+16]):
            value = word ^ key[i]
            out += struct.pack('<I', value)
            key[i] ^= value
    # Confuser uses five LZMA properties, then a FOUR-byte size, then raw data.
    props = out[0]
    lc, rest = props % 9, props // 9
    lp, pb = rest % 5, rest // 5
    size = struct.unpack_from('<I', out, 5)[0]
    dictionary = struct.unpack_from('<I', out, 1)[0]
    assert size < 16 * 1024 * 1024
    decoder = lzma.LZMADecompressor(format=lzma.FORMAT_RAW, filters=[{
        'id': lzma.FILTER_LZMA1, 'dict_size': dictionary, 'lc': lc, 'lp': lp, 'pb': pb}])
    decoded = decoder.decompress(bytes(out[9:]), max_length=size)
    assert len(decoded) == size
    return decoded

pool = unpack_array(512, 1198221968, 12)
evidence = ROOT / 'evidence/EOS-normalized'
(evidence / 'constant-pool.bin').write_bytes(pool)
embedded = unpack_array(336, 3174567445, 13)
assert embedded[:2] == b'MZ'
(evidence / 'embedded-dependency.dll').write_bytes(embedded)
parameters = {4:(1264585185,0x30f300cf), 5:(97573649,-1799865901),
              6:(-1098352681,-1462845619), 7:(781756059,-1993392034),
              8:(-344924309,0x1501092c)}
record_file=evidence / 'decoded-strings.json'
records={}
if record_file.exists():
    for row in json.loads(record_file.read_text()):
        records[f"M000{int(row['decoder_token'],16)&0xff}:{row['argument']}:{row['type']}"]=row
def decode(match):
    method, typename, identifier = int(match[1]), match[2], int(match[3])
    mul, xor = parameters[method]
    index = ((identifier * mul) ^ xor) & mask
    offset = (index & 0x3fffffff) << 2
    assert 0 <= offset < len(pool), (method, identifier, offset)
    if typename == 'string':
        length = struct.unpack_from('<I',pool,offset)[0]
        assert offset + 4 + length <= len(pool)
        value = pool[offset+4:offset+4+length].decode('utf-8')
        literal = json.dumps(value, ensure_ascii=True)
    else:
        return match[0]
    records[f'M000{method}:{identifier}:{typename}'] = {
        'decoder_token': f'0x0600000{method}', 'argument': identifier,
        'type': typename, 'pool_offset': offset, 'value': value}
    return literal

pattern = re.compile(r'(?:_003CModule_003E|<Module>)\.Method_M000([4-8])<([^>]+)>\((-?\d+)\)')
for file in SOURCE.rglob('*.cs'):
    if file.name == '-Module-.cs':
        continue
    original = file.read_text(encoding='utf-8-sig')
    file.write_text(pattern.sub(decode, original),encoding='utf-8',newline='\n')
(evidence / 'decoded-strings.json').write_text(json.dumps(list(records.values()),indent=2),encoding='utf-8',newline='\n')
print(json.dumps({'pool_bytes':len(pool),'strings':len(records),'embedded_bytes':len(embedded),
    'embedded_sha256':hashlib.sha256(embedded).hexdigest()}))
