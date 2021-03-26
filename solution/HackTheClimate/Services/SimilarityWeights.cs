namespace HackTheClimate.Services
{
    public class SimilarityWeights
    {
        public int KeywordWeight { get; set; }
        public int SectorWeight { get; set; }
        public int FrameworkWeight { get; set; }
        public int InstrumentWeight { get; set; }
        public int NaturalHazardWeight { get; set; }
        public int DocumentTypeWeight { get; set; }
        public int ResponseWeight { get; set; }
        public int LocationWeight { get; set; }
        public int TypeWeight { get; set; }
        public int EntitySkillWeight { get; set; }
        public int EntityProductWeight { get; set; }
        public int EntityEventWeight { get; set; }
        public int EntityLocationWeight { get; set; }
        public int TopicWeight { get; set; }

        public SimilarityWeights(int keywordWeight, int sectorWeight, int frameworkWeight, int instrumentWeight,
            int naturalHazardWeight, int documentTypeWeight, int responseWeight, int locationWeight, int typeWeight,
            int entitySkillWeight, int entityProductWeight, int entityEventWeight, int entityLocationWeight, int topicWeight)
        {
            KeywordWeight = keywordWeight;
            SectorWeight = sectorWeight;
            FrameworkWeight = frameworkWeight;
            InstrumentWeight = instrumentWeight;
            NaturalHazardWeight = naturalHazardWeight;
            DocumentTypeWeight = documentTypeWeight;
            ResponseWeight = responseWeight;
            LocationWeight = locationWeight;
            TypeWeight = typeWeight;
            EntitySkillWeight = entitySkillWeight;
            EntityProductWeight = entityProductWeight;
            EntityEventWeight = entityEventWeight;
            EntityLocationWeight = entityLocationWeight;
            TopicWeight = topicWeight;
        }

        public static SimilarityWeights DefaultWeights()
        {
            return new SimilarityWeights(3, 3, 3, 3, 3, 1, 3, 2, 1, 3, 4, 3, 1, 1);
        }

        public int TotalWeight()
        {
            return KeywordWeight
                   + SectorWeight
                   + FrameworkWeight
                   + InstrumentWeight
                   + NaturalHazardWeight
                   + DocumentTypeWeight
                   + ResponseWeight
                   + LocationWeight
                   + TypeWeight
                   + EntitySkillWeight
                   + EntityProductWeight
                   + EntityEventWeight
                   + EntityLocationWeight
                   + TopicWeight;
        }
    }
}
