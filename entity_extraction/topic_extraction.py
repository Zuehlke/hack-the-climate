import os
import numpy as np
import collections
import pickle
import re
import pandas as pd
from top2vec import Top2Vec
from bs4 import BeautifulSoup
from numpy import savetxt

path = "abstracts.txt"

df = pd.read_csv(path, encoding='utf8', sep=';')

def removehtml(raw_html):
    cleantext = BeautifulSoup(raw_html, "lxml").text
    cleanr = re.compile('&([a-z0-9]+|#[0-9]{1,6}|#x[0-9a-f]{1,6});')
    cleantext = re.sub(cleanr, ' ', cleantext)
    cleantext = re.sub(r'https?:\/\/.*\s', '', cleantext,
                       flags=re.MULTILINE)
    return cleantext

df = df[~df['abstract'].isna()]
df['abstract'] = df['abstract'].apply(lambda k: removehtml(k))
docs = list(df['abstract'].values)

if os.path.exists('model.dump'):
    model = pickle.load(open("model.dump", "rb"))
else:
    model = Top2Vec(docs,
                    embedding_model='distiluse-base-multilingual-cased',
                    workers=16)
    pickle.dump(model, open("model.dump", "wb"))

num_topics = model.get_num_topics()
topic_sizes, topic_nums = model.get_topic_sizes()

print(
    f'num_topics={num_topics}, topic_sizes={topic_sizes}, topic_nums={topic_nums}')

topic_words, word_scores, topic_nums = model.get_topics(np.min(num_topics))
print(topic_words[:, 0:20])

document_topic_ids, document_topic_score, document_topics_words, document_word_scores = model.get_documents_topics(
    list(range(0, len(df['abstract'].values))))

df.to_csv('baseline.csv', sep=';')
np.savetxt('topic_words.txt', topic_words, fmt='%s')
np.savetxt('word_scores.txt', word_scores)
np.savetxt('topic_nums.txt', topic_nums)
np.savetxt('document_topic_ids.txt', document_topic_ids)
np.savetxt('document_topic_score.txt', document_topic_score)
np.savetxt('document_topics_words.txt', document_topics_words, fmt='%s')
np.savetxt('document_word_scores.txt', document_word_scores)
