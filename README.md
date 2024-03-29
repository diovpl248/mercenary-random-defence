용병 랜덤 디펜스 (소스 참고용)
---

![메인화면](https://github.com/diovpl248/mercenary-random-defence/blob/master/game.png?raw=true)

![인게임](https://github.com/diovpl248/mercenary-random-defence/blob/master/in-game.PNG?raw=true)

---

> 팀 프로젝트

- 제작 인원: 5명

> 개발 환경

- 개발 툴: Unity 2018, Visual Studio 2017
- 개발 언어: C#
- 플랫폼: Android

> 제작 기간
- 2018.11 ~ 2019.03 (4개월)

> 역할
- 팀장
- 주요 UI 구성(메인메뉴, 뽑기, 조합결과, 유물선택, 퀵슬롯 등)
- 주요 모듈 개발(캐릭터 클래스, 전투 로직, 토템 클래스, 유물 클래스, 퀘스트 등)
- 부가 시스템 개발(세이브&로드, JSON데이터 로드)

---

* [게임 데이터] 유닛기본스텟, 토템, 유물, 조합식 등 json파일에서 읽어옴. JsonManager.cs, (Resources\Json폴더) 
* [타원] Ellipse.cs
* [캐릭터] Character.cs
* [이동] Movement.cs
* [공격] Attack.cs
* [특수 스킬 공격] Magic.cs, ExplosionParticle.cs
* [토템] TotemManager.cs, Totem.cs
* [유물] ArtifactManager.cs
* [버프 디버프] 공격력상승, 방어력상승, 힐, 도트데미지, 슬로우 등을 적용 BuffSystem.cs
* [최적화] 유닛, 데미지폰트, 스킬이펙트등 오브젝트 풀링으로 재사용 MasterFloatingTextPool.cs, MasterObjectPool.cs
* [옵저버 패턴을 활용 c# delegate] QuestEventManager.cs
* [세이브 로드] 바이너리 파일로 세이브 및 로드 SaveSysttem.cs
* [UI] Async씬로딩 Loading.cs


YouTube: https://youtu.be/KK5aYnqKmR0