from collections import Counter
from pathlib import Path
import re

TEXT_DIR = Path(__file__).resolve().parent


def tokenize(text: str) -> list[str]:
    return re.findall(r"[A-Za-z']+", text.lower())


def build_word_stats(folder: Path) -> Counter[str]:
    counter: Counter[str] = Counter()
    for path in folder.glob("*.txt"):
        content = path.read_text(encoding="utf-8", errors="ignore")
        counter.update(tokenize(content))
    return counter


def main() -> None:
    stats = build_word_stats(TEXT_DIR)
    print("Top 10 words in .txt files:")
    for word, count in stats.most_common(10):
        print(f"{word:>12} : {count}")


if __name__ == "__main__":
    main()
