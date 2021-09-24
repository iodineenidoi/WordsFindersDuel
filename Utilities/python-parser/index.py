import codecs
import requests
from bs4 import BeautifulSoup

headers = {
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36",
    "Accept": "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9",
}

letters = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ"


def get_url(letter, page):
    return f"https://wordsonline.ru/{letter}?page={page}"


def download_page(letter, page):
    url = get_url(letter, page)
    r = requests.get(url=url, headers=headers)
    return r.text


def get_page_soup(page_data):
    return BeautifulSoup(page_data, "html.parser")


def get_words_from_page(page_data, letter):
    soup = get_page_soup(page_data)

    if not soup:
        return []

    res = []
    for link in soup.find_all('li'):
        if link.a and link.a.get('href') == f"/{letter}/{link.a.string}":
            res.append(link.a.string)

    return res


def download_all_letter_pages_and_save_words_to_file(letter):
    current_page_index = 1
    letter_pages = []
    while True:
        print(f"Download page: {current_page_index} for letter: {letter}")
        page_data = download_page(letter, current_page_index)
        if page_data:
            current_page_index += 1
            letter_pages.append(page_data)
        else:
            break
    
    f = codecs.open(f"results/{letter}.txt", "w", "utf-8-sig")

    for page_data in letter_pages:
        words = get_words_from_page(page_data, letter)
        for word in words:
            f.write(f"{word}\n")

    f.close()


def main():
    for letter in letters:
        download_all_letter_pages_and_save_words_to_file(letter)


if __name__ == "__main__":
    main()