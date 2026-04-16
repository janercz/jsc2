from datetime import datetime
from pathlib import Path
import json


def collect_txt_sizes(folder: Path) -> list[dict[str, object]]:
    rows: list[dict[str, object]] = []
    for p in sorted(folder.glob('*.txt')):
        rows.append(
            {
                'name': p.name,
                'bytes': p.stat().st_size,
                'updated_utc': datetime.utcnow().isoformat(timespec='seconds') + 'Z',
            }
        )
    return rows


def main() -> None:
    root = Path(__file__).resolve().parent
    data = collect_txt_sizes(root)
    out_file = root / 'txt_sizes.json'
    out_file.write_text(json.dumps(data, indent=2), encoding='utf-8')
    print(f'Written {out_file.name} with {len(data)} entries')


if __name__ == '__main__':
    main()
